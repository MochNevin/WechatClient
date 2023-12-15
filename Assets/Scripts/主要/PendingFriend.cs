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
    ///     待处理好友类，用于表示待处理的好友请求
    /// </summary>
    public sealed class PendingFriend : MonoBehaviour
    {
        /// <summary>
        ///     待处理好友的名称显示文本
        /// </summary>
        public TMP_Text Name;

        /// <summary>
        ///     接受好友请求的按钮组件
        /// </summary>
        public Button AcceptButton;

        /// <summary>
        ///     拒绝好友请求的按钮组件
        /// </summary>
        public Button RejectButton;

        /// <summary>
        ///     在对象被唤醒时调用的方法，设置接受和拒绝按钮的点击事件监听器
        /// </summary>
        private void Awake()
        {
            AcceptButton.onClick.AddListener(Accept);
            RejectButton.onClick.AddListener(Reject);
        }

        /// <summary>
        ///     接受好友请求的方法
        /// </summary>
        public void Accept() => NetworkTransport.Singleton.Send(new MessageUpdateFriend
        {
            username = Name.text,
            accepted = true
        });

        /// <summary>
        ///     拒绝好友请求的方法
        /// </summary>
        public void Reject() => NetworkTransport.Singleton.Send(new MessageUpdateFriend
        {
            username = Name.text,
            accepted = false
        });
    }
}