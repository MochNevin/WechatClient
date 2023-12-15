//------------------------------------------------------------
// 偷我的代码就会被拖进黑暗空间
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Erinn
{
    /// <summary>
    ///     好友类，用于表示聊天中的好友对象
    /// </summary>
    public sealed class Friend : MonoBehaviour
    {
        /// <summary>
        ///     好友的名称显示文本
        /// </summary>
        public TMP_Text Name;

        /// <summary>
        ///     用于触发聊天按钮的按钮组件
        /// </summary>
        public Button chatButton;

        /// <summary>
        ///     获取主面板中的聊天名称文本组件
        /// </summary>
        public TMP_Text ChatName => MainPanel.Singleton.ChatName;

        /// <summary>
        ///     获取主面板中的聊天面板组件
        /// </summary>
        public ChatPanel chatPanel => MainPanel.Singleton.chatPanel;

        /// <summary>
        ///     在对象被唤醒时调用的方法，设置聊天按钮的点击事件监听器
        /// </summary>
        private void Awake() => chatButton.onClick.AddListener(Chat);

        /// <summary>
        ///     触发聊天的方法
        /// </summary>
        public void Chat()
        {
            ChatName.text = Name.text;
            chatPanel.Chat();
            MainPanel.Singleton.ShowChat();
        }
    }
}