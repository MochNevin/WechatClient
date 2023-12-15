//------------------------------------------------------------
// 偷我的代码就会被拖进黑暗空间
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Erinn
{
    /// <summary>
    ///     表示一个聊天面板的类，用于管理和显示聊天视图
    /// </summary>
    public sealed class ChatPanel : MonoBehaviour
    {
        /// <summary>
        ///     当前活动的聊天视图
        /// </summary>
        [HideInInspector] public ChatView Active;

        /// <summary>
        ///     聊天面板的名称文本
        /// </summary>
        [LabelText("名称")] public TMP_Text ChatName;

        /// <summary>
        ///     聊天视图的预制件
        /// </summary>
        [LabelText("预制件")] public ChatView Prefab;

        /// <summary>
        ///     聊天视图的对象池
        /// </summary>
        [LabelText("池")] public Transform Pool;

        /// <summary>
        ///     存储用户和对应聊天视图的字典
        /// </summary>
        public Dictionary<string, ChatView> ChatViews;

        /// <summary>
        ///     在对象被唤醒时调用，用于初始化字典
        /// </summary>
        public void OnAwake() => ChatViews = new Dictionary<string, ChatView>();

        /// <summary>
        ///     接收用户的消息并在对应的聊天视图中显示
        /// </summary>
        /// <param name="username">发送消息的用户名</param>
        /// <param name="message">接收到的消息内容</param>
        public void Receive(string username, string message)
        {
            if (!ChatViews.TryGetValue(username, out var chatView))
            {
                chatView = Instantiate(Prefab, Pool, false);
                chatView.Name = username;
                ChatViews[username] = chatView;
            }

            chatView.Receive(message);
        }

        /// <summary>
        ///     切换到指定用户的聊天视图或创建新的视图
        /// </summary>
        public void Chat()
        {
            if (Active != null)
            {
                if (Active.Name == ChatName.text)
                    return;
                Active.gameObject.SetActive(false);
            }

            if (!ChatViews.TryGetValue(ChatName.text, out var chatView))
            {
                chatView = Instantiate(Prefab, Pool, false);
                chatView.Name = ChatName.text;
                ChatViews[ChatName.text] = chatView;
            }

            Active = chatView;
            chatView.gameObject.SetActive(true);
        }
    }
}