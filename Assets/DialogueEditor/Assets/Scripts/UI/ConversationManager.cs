using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

namespace DialogueEditor
{
    public class ConversationManager : MonoBehaviour
    {
        private enum eState
        {
            TransitioningDialogueBoxOn,
            ScrollingText,
            TransitioningOptionsOn,
            Idle,
            TransitioningOptionsOff,
            TransitioningDialogueOff,
            Off,
            NONE,
        }

        private const float TRANSITION_TIME = 0.2f; // Transition time for fades

        public static ConversationManager Instance { get; private set; }

        public delegate void ConversationStartEvent();
        public delegate void ConversationEndEvent();

        public static ConversationStartEvent OnConversationStarted;
        public static ConversationEndEvent OnConversationEnded;

        public bool ScrollText;
        public float ScrollSpeed = 1;
        public Sprite BackgroundImage;
        public bool BackgroundImageSliced;
        public Sprite OptionImage;
        public bool OptionImageSliced;
        public bool AllowMouseInteraction;

        public RectTransform DialoguePanel;
        public RectTransform OptionsPanel;
        public Image DialogueBackground;
        public Image NpcIcon;
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI DialogueText;
        public AudioSource AudioPlayer;
        public UIConversationButton ButtonPrefab;
        public Sprite BlankSprite;

        public bool IsConversationActive => m_state != eState.NONE && m_state != eState.Off;

        private float m_elapsedScrollTime;
        private int m_scrollIndex;
        public int m_targetScrollTextCount;
        private eState m_state;
        private float m_stateTime;

        private Conversation m_conversation;
        private SpeechNode m_currentSpeech;
        private OptionNode m_selectedOption;
        private List<UIConversationButton> m_uiOptions;
        private int m_currentSelectedIndex;

        private NPCConversation npcConversation;
        private PlayerController playerController;
        private bool m_dialogueFinishedScrolling = false;
        //private bool m_conversationEnding = false;
        private bool m_showingOption = false;
        private float BUTTON_COOLDOWN = 2f; // 2-second cooldown for button presses
        private bool isConversationActive = false;
        //private bool endingConversation = false;

        private bool canProcessClick = true;
        private Coroutine fadeCoroutine;

        public event Action OnDialogueEnd;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            Instance = this;

            m_uiOptions = new List<UIConversationButton>();
            NpcIcon.sprite = BlankSprite;
            DialogueText.text = "";
            TurnOffUI();

            npcConversation = FindObjectOfType<NPCConversation>();
            playerController = FindObjectOfType<PlayerController>();
            BUTTON_COOLDOWN = 0;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void Update()
        {
            Debug.Log($"m_dialogueFinishedScrolling set to: {m_dialogueFinishedScrolling}");
            // Handle NPC image visibility based on scrolling state
            BUTTON_COOLDOWN = Mathf.Max(0, BUTTON_COOLDOWN - Time.deltaTime);

            // Handle left mouse button click and state transitions
            if (Input.GetMouseButtonDown(0) && m_dialogueFinishedScrolling && !m_showingOption && isConversationActive && canProcessClick)
            {
                // Start cooldown period
                canProcessClick = false;

                switch (m_state)
                {
                    case eState.TransitioningDialogueBoxOn:
                    case eState.ScrollingText:
                    case eState.TransitioningOptionsOn:
                    case eState.Idle:
                        SpeechNode nextSpeech = GetValidSpeechOfNode(m_currentSpeech);
                        if (nextSpeech != null)
                        {
                            if (playerController != null)
                            {
                                playerController.ToggleMovement(false);
                            }
                            //DisableCursor();
                            SetupSpeech(nextSpeech);
                        }
                        else
                        {
                            EndConversation();
                            //endingConversation = false;
                        }
                        break;

                    case eState.TransitioningOptionsOff:
                    case eState.TransitioningDialogueOff:
                        break;

                    default:
                        break;
                }
            }

            // Call existing logic based on the state if not left-clicked
            switch (m_state)
            {
                case eState.TransitioningDialogueBoxOn:
                    TransitioningDialogueBoxOn_Update();
                    break;

                case eState.ScrollingText:
                    ScrollingText_Update();
                    break;

                case eState.TransitioningOptionsOn:
                    TransitionOptionsOn_Update();
                    break;

                case eState.Idle:
                    Idle_Update();
                    break;

                case eState.TransitioningOptionsOff:
                    TransitionOptionsOff_Update();
                    break;

                case eState.TransitioningDialogueOff:
                    TransitioningDialogueBoxOff_Update();
                    break;

                default:
                    break;
            }
        }

        public void DisableAllColliders()
        {
            Collider[] allColliders = FindObjectsOfType<Collider>();
            foreach (Collider collider in allColliders)
            {
                collider.enabled = false;
                Debug.Log("disable colliders");
            }
        }

        public void EnableAllColliders()
        {
            Collider[] allColliders = FindObjectsOfType<Collider>();
            foreach (Collider collider in allColliders)
            {
                collider.enabled = true;
                Debug.Log("enable colliders");
            }
        }

        private bool IsDreamScene()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            return sceneName.Contains("Dream");
        }

        private void ResetClickProcessing()
        {
            canProcessClick = true;
        }


        void DisableCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        void EnableCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }


        //--------------------------------------
        // Public functions
        //--------------------------------------

        public void StartConversation(NPCConversation conversation)
        {
            Debug.Log("Starting conversation");

            //DisableCursor();
            GameManager.Instance.TogglePauseInput(false);
            playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.ToggleMovement(false);
                playerController.ToggleAttacks(false);
            }


            if (IsDreamScene())
            {
                DisableAllColliders();
            }

            isConversationActive = true;

            m_conversation = conversation.Deserialize();
            if (OnConversationStarted != null)
                OnConversationStarted.Invoke();

            TurnOnUI();
            m_currentSpeech = m_conversation.Root;
            SetState(eState.TransitioningDialogueBoxOn);
        }

        public void EndConversation()
        {
            SetState(eState.TransitioningDialogueOff);

            if (OnConversationEnded != null)
                OnConversationEnded.Invoke();
            OnDialogueEnd?.Invoke();
            //m_conversationEnding = false;
            isConversationActive = false;
            Debug.Log("Ending conversation");

            if (IsDreamScene())
            {
                EnableAllColliders();
            }


            //endingConversation = true;

            //EnableCursor();
            GameManager.Instance.TogglePauseInput(true);
            if (playerController != null)
            {
                playerController.ToggleMovement(true);
                playerController.ToggleAttacks(true);
            }
        }

        public void SelectNextOption()
        {
            int next = m_currentSelectedIndex + 1;
            if (next > m_uiOptions.Count - 1)
            {
                next = 0;
            }
            SetSelectedOption(next);
        }

        public void SelectPreviousOption()
        {
            int previous = m_currentSelectedIndex - 1;
            if (previous < 0)
            {
                previous = m_uiOptions.Count - 1;
            }
            SetSelectedOption(previous);
        }

        public void PressSelectedOption()
        {
            // If the cooldown has expired, proceed with the action
            if (m_state != eState.Idle) { return; }
            if (m_currentSelectedIndex < 0) { return; }
            if (m_currentSelectedIndex >= m_uiOptions.Count) { return; }
            if (m_uiOptions.Count == 0) { return; }

            UIConversationButton button = m_uiOptions[m_currentSelectedIndex];
            button.OnButtonPressed();
            //DisableCursor();
        }

        public void AlertHover(UIConversationButton button)
        {
            for (int i = 0; i < m_uiOptions.Count; i++)
            {
                if (m_uiOptions[i] == button && m_currentSelectedIndex != i)
                {
                    SetSelectedOption(i);
                    return;
                }
            }

            if (button == null)
                UnselectOption();
        }

        public void SetInt(string paramName, int value)
        {
            eParamStatus status;
            m_conversation.SetInt(paramName, value, out status);

            if (status == eParamStatus.NoParamFound)
            {
                LogWarning("parameter \'" + paramName + "\' does not exist.");
            }
        }

        public void SetBool(string paramName, bool value)
        {
            eParamStatus status;
            m_conversation.SetBool(paramName, value, out status);

            if (status == eParamStatus.NoParamFound)
            {
                LogWarning("parameter \'" + paramName + "\' does not exist.");
            }
        }

        public int GetInt(string paramName)
        {
            eParamStatus status;
            int value = m_conversation.GetInt(paramName, out status);

            if (status == eParamStatus.NoParamFound)
            {
                LogWarning("parameter \'" + paramName + "\' does not exist.");
            }

            return value;
        }

        public bool GetBool(string paramName)
        {
            eParamStatus status;
            bool value = m_conversation.GetBool(paramName, out status);

            if (status == eParamStatus.NoParamFound)
            {
                LogWarning("parameter \'" + paramName + "\' does not exist.");
            }

            return value;
        }


        //--------------------------------------
        // Set state
        //--------------------------------------

        private void SetState(eState newState)
        {
            // Exit
            switch (m_state)
            {
                case eState.TransitioningOptionsOff:
                    m_selectedOption = null;
                    break;
                case eState.TransitioningDialogueBoxOn:
                    SetColorAlpha(DialogueBackground, 1);
                    SetColorAlpha(NpcIcon, 1);
                    SetColorAlpha(NameText, 1);
                    break;
            }

            m_state = newState;
            m_stateTime = 0f;

            // Enter 
            switch (m_state)
            {
                case eState.TransitioningDialogueBoxOn:
                    {
                        SetColorAlpha(DialogueBackground, 0);
                        SetColorAlpha(NpcIcon, 0);
                        SetColorAlpha(NameText, 0);

                        DialogueText.text = "";
                        NameText.text = m_currentSpeech.Name;
                        NpcIcon.sprite = m_currentSpeech.Icon != null ? m_currentSpeech.Icon : BlankSprite;
                    }
                    break;

                case eState.ScrollingText:
                    {
                        SetColorAlpha(DialogueText, 1);
                    }
                    break;

                case eState.TransitioningOptionsOn:
                    {
                        SetColorAlpha(DialogueText, 1);

                        CreateUIOptions();

                        for (int i = 0; i < m_uiOptions.Count; i++)
                        {
                            m_uiOptions[i].gameObject.SetActive(true);
                        }
                    }
                    break;
            }
        }




        //--------------------------------------
        // Update
        //--------------------------------------

        private void TransitioningDialogueBoxOn_Update()
        {
            m_stateTime += Time.deltaTime;
            float t = m_stateTime / TRANSITION_TIME;

            if (t > 1)
            {
                SetupSpeech(m_currentSpeech);
                return;
            }

            SetColorAlpha(DialogueBackground, t);
            SetColorAlpha(NpcIcon, t);
            SetColorAlpha(NameText, t);
        }

        // Modify the ScrollingText_Update method to set the flag when the dialogue finishes scrolling
        private void ScrollingText_Update()
        {
            const float charactersPerSecond = 1500;
            float timePerChar = (60.0f / charactersPerSecond);
            timePerChar *= ScrollSpeed;

            m_elapsedScrollTime += Time.deltaTime;

            if (m_elapsedScrollTime > timePerChar)
            {
                m_elapsedScrollTime = 0f;

                DialogueText.maxVisibleCharacters = m_scrollIndex;
                m_scrollIndex++;

                // Finished scrolling?
                if (m_scrollIndex >= m_targetScrollTextCount)
                {
                    ResetClickProcessing();
                    // Set the flag to indicate that the dialogue has finished scrolling
                    m_dialogueFinishedScrolling = true;

                    // Activate the GameObject with the tag "NPCImage"
                    /*GameObject npcImage = GameObject.FindWithTag("NPCImage");
                    if (npcImage != null)
                    {
                        npcImage.SetActive(true);

                        // Start the fade in/out coroutine
                        if (fadeCoroutine != null)
                        {
                            StopCoroutine(fadeCoroutine);
                        }
                        fadeCoroutine = StartCoroutine(FadeInOut(npcImage.GetComponent<CanvasGroup>()));
                    }*/

                    // Find the parent GameObject with the tag "NPCImage"


                    // Automatically transition to options once scrolling is finished
                    SetState(eState.TransitioningOptionsOn);
                }
            }
        }

        private IEnumerator FadeInOut(CanvasGroup canvasGroup)
        {
            float duration = 1.5f; // Time to fade in or out
            float alpha = 0;
            bool fadingIn = true;

            while (m_dialogueFinishedScrolling && !m_showingOption)
            {
                while (fadingIn)
                {
                    alpha += Time.deltaTime / duration;
                    canvasGroup.alpha = Mathf.Clamp01(alpha);

                    if (alpha >= 1)
                    {
                        fadingIn = false;
                    }
                    yield return null;
                }

                while (!fadingIn)
                {
                    alpha -= Time.deltaTime / duration;
                    canvasGroup.alpha = Mathf.Clamp01(alpha);

                    if (alpha <= 0)
                    {
                        fadingIn = true;
                    }
                    yield return null;
                }
            }

            // Reset alpha when the coroutine stops
            canvasGroup.alpha = 0;
        }


        private void TransitionOptionsOn_Update()
        {
            m_stateTime += Time.deltaTime;
            float t = m_stateTime / TRANSITION_TIME;

            if (t > 1)
            {
                SetState(eState.Idle);
                return;
            }

            for (int i = 0; i < m_uiOptions.Count; i++)
                m_uiOptions[i].SetAlpha(t);
        }

        private void Idle_Update()
        {
            m_stateTime += Time.deltaTime;

            if (m_currentSpeech.AutomaticallyAdvance)
            {
                if (m_currentSpeech.ConnectionType == Connection.eConnectionType.None || m_currentSpeech.ConnectionType == Connection.eConnectionType.Speech)
                {
                    if (m_stateTime > m_currentSpeech.TimeUntilAdvance)
                    {
                        SetState(eState.TransitioningOptionsOff);
                    }
                }
            }
        }

        private void TransitionOptionsOff_Update()
        {
            m_stateTime += Time.deltaTime;
            float t = m_stateTime / TRANSITION_TIME;

            if (t > 1)
            {
                ClearOptions();

                if (m_currentSpeech.AutomaticallyAdvance)
                {
                    if (IsAutoAdvance())
                        return;
                }

                if (m_selectedOption == null)
                {
                    EndConversation();
                    return;
                }

                SpeechNode nextSpeech = GetValidSpeechOfNode(m_selectedOption);
                if (nextSpeech == null)
                {
                    EndConversation();
                }
                else
                {
                    SetupSpeech(nextSpeech);
                }
                return;
            }


            for (int i = 0; i < m_uiOptions.Count; i++)
                m_uiOptions[i].SetAlpha(1 - t);

            SetColorAlpha(DialogueText, 1 - t);
        }

        private void TransitioningDialogueBoxOff_Update()
        {
            m_stateTime += Time.deltaTime;
            float t = m_stateTime / TRANSITION_TIME;

            if (t > 1)
            {
                TurnOffUI();
                return;
            }

            SetColorAlpha(DialogueBackground, 1 - t);
            SetColorAlpha(NpcIcon, 1 - t);
            SetColorAlpha(NameText, 1 - t);
        }



        //--------------------------------------
        // Do Speech
        //--------------------------------------

        private void SetupSpeech(SpeechNode speech)
        {
            // Debug.Log("set up speech");
            // Reset the flag at the beginning of the method
            m_dialogueFinishedScrolling = false;

            if (speech == null)
            {
                EndConversation();
                return;
            }

            m_currentSpeech = speech;

            // Clear current options
            ClearOptions();
            m_currentSelectedIndex = 0;

            // Set sprite
            if (speech.Icon == null)
            {
                NpcIcon.sprite = BlankSprite;
            }
            else
            {
                NpcIcon.sprite = speech.Icon;
            }

            // Set font
            if (speech.TMPFont != null)
            {
                DialogueText.font = speech.TMPFont;
            }
            else
            {
                DialogueText.font = null;
            }

            // Set name
            NameText.text = speech.Name;

            // Set text
            if (string.IsNullOrEmpty(speech.Text))
            {
                if (ScrollText)
                {
                    DialogueText.text = "";
                    m_targetScrollTextCount = 0;
                    DialogueText.maxVisibleCharacters = 0;
                    m_elapsedScrollTime = 0f;
                    m_scrollIndex = 0;
                }
                else
                {
                    DialogueText.text = "";
                    DialogueText.maxVisibleCharacters = 1;
                }
            }
            else
            {
                if (ScrollText)
                {
                    DialogueText.text = speech.Text;
                    m_targetScrollTextCount = speech.Text.Length + 1;
                    DialogueText.maxVisibleCharacters = 0;
                    m_elapsedScrollTime = 0f;
                    m_scrollIndex = 0;
                }
                else
                {
                    DialogueText.text = speech.Text;
                    DialogueText.maxVisibleCharacters = speech.Text.Length;
                }
            }

            // Call the event
            if (speech.Event != null)
                speech.Event.Invoke();

            DoParamAction(speech);

            // Play the audio
            if (speech.Audio != null)
            {
                AudioPlayer.clip = speech.Audio;
                AudioPlayer.volume = speech.Volume;
                AudioPlayer.Play();
            }

            // Set the dialogue active flag and disable the component if the next node is a speech node
            if (GetValidSpeechOfNode(speech) != null)
            {
                npcConversation.isDialogueActive = true;

            }

            if (ScrollText)
            {
                SetState(eState.ScrollingText);
            }
            else
            {
                SetState(eState.TransitioningOptionsOn);
            }
        }






        //--------------------------------------
        // Option Selected
        //--------------------------------------

        public void SpeechSelected(SpeechNode speech)
        {
            SetupSpeech(speech);
        }

        public void OptionSelected(OptionNode option)
        {
            if (BUTTON_COOLDOWN <= 0)
            {
                Debug.Log("selected options");
                m_dialogueFinishedScrolling = false;
                m_showingOption = false;
                BUTTON_COOLDOWN = 2;
                // Update the last button press time
                // BUTTON_COOLDOWN -= Time.time;
                m_selectedOption = option;
                DoParamAction(option);
                if (option.Event != null)
                    option.Event.Invoke();
                SetState(eState.TransitioningOptionsOff);
                //DisableCursor();
            }

            else
            {
                // If the cooldown has not expired, ignore the button press
                Debug.Log("Button press cooldown in effect. Please wait.");
            }
        }

        public void EndButtonSelected()
        {
            m_selectedOption = null;
            SetState(eState.TransitioningOptionsOff);
        }




        //--------------------------------------
        // Util
        //--------------------------------------

        private bool IsAutoAdvance()
        {
            if (m_currentSpeech.ConnectionType == Connection.eConnectionType.Speech)
            {
                SpeechNode next = GetValidSpeechOfNode(m_currentSpeech);
                if (next != null)
                {
                    SetupSpeech(next);
                    return true;
                }
            }
            else if (m_currentSpeech.ConnectionType == Connection.eConnectionType.None)
            {
                EndConversation();
                return true;
            }
            return false;
        }

        /// <summary> Returns the first, valid child connection to a Speech Node. </summary>
        private SpeechNode GetValidSpeechOfNode(ConversationNode parentNode)
        {
            if (parentNode.ConnectionType != Connection.eConnectionType.Speech) { return null; }
            if (parentNode.Connections.Count == 0) { return null; }

            // Loop through connections, until a valid connection is found.
            for (int i = 0; i < parentNode.Connections.Count; i++)
            {
                SpeechConnection connection = parentNode.Connections[i] as SpeechConnection;
                bool conditionsMet = ConditionsMet(connection);

                if (conditionsMet)
                {
                    return connection.SpeechNode;
                }
            }

            return null;
        }

        private void TurnOnUI()
        {
            DialoguePanel.gameObject.SetActive(true);
            OptionsPanel.gameObject.SetActive(true);

            if (BackgroundImage != null)
            {
                DialogueBackground.sprite = BackgroundImage;

                if (BackgroundImageSliced)
                    DialogueBackground.type = Image.Type.Sliced;
                else
                    DialogueBackground.type = Image.Type.Simple;
            }

            NpcIcon.sprite = BlankSprite;
        }

        private void TurnOffUI()
        {
            DialoguePanel.gameObject.SetActive(false);
            OptionsPanel.gameObject.SetActive(false);
            SetState(eState.Off);
#if UNITY_EDITOR
            // Debug.Log("[ConversationManager]: Conversation UI off.");
#endif
        }

        private void CreateUIOptions()
        {
            // Display new options
            if (m_currentSpeech.ConnectionType == Connection.eConnectionType.Option)
            {
                Debug.Log("create options");
                EnableCursor();
                m_showingOption = true;
                for (int i = 0; i < m_currentSpeech.Connections.Count; i++)
                {
                    OptionConnection connection = m_currentSpeech.Connections[i] as OptionConnection;
                    if (ConditionsMet(connection))
                    {
                        UIConversationButton uiOption = CreateButton();
                        uiOption.SetupButton(UIConversationButton.eButtonType.Option, connection.OptionNode);
                    }
                }
            }


            // Display Continue/End options
            /* else
            {
                bool notAutoAdvance = !m_currentSpeech.AutomaticallyAdvance;
                bool allowVisibleOptionWithAuto = (m_currentSpeech.AutomaticallyAdvance && m_currentSpeech.AutoAdvanceShouldDisplayOption);

                if (notAutoAdvance || allowVisibleOptionWithAuto)
                {
                    if (m_currentSpeech.ConnectionType == Connection.eConnectionType.Speech)
                    {
                        UIConversationButton uiOption = CreateButton();
                        SpeechNode next = GetValidSpeechOfNode(m_currentSpeech);

                        // If there was no valid speech node (due to no conditions being met) this becomes a None button type
                        if (next == null)
                        {
                            uiOption.SetupButton(UIConversationButton.eButtonType.End, null, endFont: m_conversation.EndConversationFont);
                        }
                        // Else, valid speech node found
                        else
                        {
                            uiOption.SetupButton(UIConversationButton.eButtonType.Speech, next, continueFont: m_conversation.ContinueFont);
                        }
                        
                    }
                    else if (m_currentSpeech.ConnectionType == Connection.eConnectionType.None)
                    {
                        UIConversationButton uiOption = CreateButton();
                        uiOption.SetupButton(UIConversationButton.eButtonType.End, null, endFont: m_conversation.EndConversationFont);
                    }
                }

            }
            */
            SetSelectedOption(0);

            // Set the button sprite and alpha
            for (int i = 0; i < m_uiOptions.Count; i++)
            {
                m_uiOptions[i].SetImage(OptionImage, OptionImageSliced);
                m_uiOptions[i].SetAlpha(0);
                m_uiOptions[i].gameObject.SetActive(false);
            }
        }

        private void ClearOptions()
        {
            while (m_uiOptions.Count != 0)
            {
                GameObject.Destroy(m_uiOptions[0].gameObject);
                m_uiOptions.RemoveAt(0);
            }
        }

        private void SetColorAlpha(MaskableGraphic graphic, float a)
        {
            Color col = graphic.color;
            col.a = a;
            graphic.color = col;
        }

        private void SetSelectedOption(int index)
        {
            if (m_uiOptions.Count == 0) { return; }

            if (index < 0)
                index = 0;
            if (index > m_uiOptions.Count - 1)
                index = m_uiOptions.Count - 1;

            if (m_currentSelectedIndex >= 0)
                m_uiOptions[m_currentSelectedIndex].SetHovering(false);
            m_currentSelectedIndex = index;
            m_uiOptions[index].SetHovering(true);
        }

        private void UnselectOption()
        {
            if (m_currentSelectedIndex < 0) { return; }

            m_uiOptions[m_currentSelectedIndex].SetHovering(false);
            m_currentSelectedIndex = -1;
        }

        private UIConversationButton CreateButton()
        {
            UIConversationButton button = GameObject.Instantiate(ButtonPrefab, OptionsPanel);
            m_uiOptions.Add(button);
            EnableCursor();
            return button;
        }

        private bool ConditionsMet(Connection connection)
        {
            List<Condition> conditions = connection.Conditions;
            for (int i = 0; i < conditions.Count; i++)
            {
                bool conditionMet = false;

                // Int condition
                if (conditions[i].ConditionType == Condition.eConditionType.IntCondition)
                {
                    IntCondition condition = conditions[i] as IntCondition;

                    string paramName = condition.ParameterName;
                    int requiredValue = condition.RequiredValue;
                    eParamStatus status;
                    int currentValue = m_conversation.GetInt(paramName, out status);

                    switch (condition.CheckType)
                    {
                        case IntCondition.eCheckType.equal:
                            conditionMet = (currentValue == requiredValue);
                            break;

                        case IntCondition.eCheckType.greaterThan:
                            conditionMet = (currentValue > requiredValue);
                            break;

                        case IntCondition.eCheckType.lessThan:
                            conditionMet = (currentValue < requiredValue);
                            break;
                    }
                }
                // Bool condition
                if (conditions[i].ConditionType == Condition.eConditionType.BoolCondition)
                {
                    BoolCondition condition = conditions[i] as BoolCondition;

                    string paramName = condition.ParameterName;
                    bool requiredValue = condition.RequiredValue;
                    eParamStatus status;
                    bool currentValue = m_conversation.GetBool(paramName, out status);

                    switch (condition.CheckType)
                    {
                        case BoolCondition.eCheckType.equal:
                            conditionMet = (currentValue == requiredValue);
                            break;

                        case BoolCondition.eCheckType.notEqual:
                            conditionMet = (currentValue != requiredValue);
                            break;
                    }
                }

                if (!conditionMet)
                {
                    return false;
                }
            }

            return true;
        }

        public void DoParamAction(ConversationNode node)
        {
            if (node.ParamActions == null) { return; }

            for (int i = 0; i < node.ParamActions.Count; i++)
            {
                string name = node.ParamActions[i].ParameterName;

                if (node.ParamActions[i].ParamActionType == SetParamAction.eParamActionType.Int)
                {
                    int val = (node.ParamActions[i] as SetIntParamAction).Value;
                    SetInt(name, val);
                }
                else if (node.ParamActions[i].ParamActionType == SetParamAction.eParamActionType.Bool)
                {
                    bool val = (node.ParamActions[i] as SetBoolParamAction).Value;
                    SetBool(name, val);
                }
            }
        }

        private void LogWarning(string warning)
        {
#if UNITY_EDITOR
            Debug.LogWarning("[Dialogue Editor]: " + warning);
#endif
        }
    }
}