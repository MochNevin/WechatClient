//------------------------------------------------------------
// 偷我的代码就会被拖进黑暗空间
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Erinn
{
    /// <summary>
    ///     聊天视图类，用于管理聊天界面的显示与消息发送
    /// </summary>
    public class ChatView : MonoBehaviour
    {
        /// <summary>
        ///     用户名，隐藏在Inspector面板中
        /// </summary>
        [HideInInspector] public string Name;

        /// <summary>
        ///     输入框，用于用户输入消息
        /// </summary>
        [LabelText("输入框")] public TMP_InputField InputField;

        /// <summary>
        ///     左侧聊天框，用于显示接收到的消息
        /// </summary>
        [LabelText("左")] public ChatGrid Left;

        /// <summary>
        ///     右侧聊天框，用于显示发送的消息
        /// </summary>
        [LabelText("右")] public ChatGrid Right;

        /// <summary>
        ///     聊天框对象池，用于聊天框的对象池管理
        /// </summary>
        [LabelText("池")] public Transform Pool;

        /// <summary>
        ///     发送按钮，用户点击发送消息
        /// </summary>
        public Button SendButton;

        /// <summary>
        ///     在对象被唤醒时调用的方法，设置发送按钮的点击事件监听器
        /// </summary>
        private void Awake() => SendButton.onClick.AddListener(Send);

        /// <summary>
        ///     发送消息的方法
        /// </summary>
        public void Send()
        {
            if (string.IsNullOrEmpty(InputField.text))
                return;
            NetworkTransport.Singleton.Send(new MessageChat
            {
                username = Name,
                content = InputField.text
            });
            var rightChatGrid = Instantiate(Right.gameObject, Pool, false).GetComponent<ChatGrid>();
            rightChatGrid.TMPText.text = InputField.text;
            InputField.text = "";
        }

        /// <summary>
        ///     接收消息的方法
        /// </summary>
        /// <param name="message">接收到的消息文本</param>
        public void Receive(string message)
        {
            var leftChatGrid = Instantiate(Left.gameObject, Pool, false).GetComponent<ChatGrid>();
            leftChatGrid.TMPText.text = message;
        }
    }
}