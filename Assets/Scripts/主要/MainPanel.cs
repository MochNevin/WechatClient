//------------------------------------------------------------
// 偷我的代码就会被拖进黑暗空间
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using System;
using System.Net;
using Cysharp.Threading.Tasks;
using MemoryPack;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Erinn
{
    /// <summary>
    ///     主面板类，用于管理聊天客户端的主界面和相关操作
    /// </summary>
    public sealed class MainPanel : MonoSingleton<MainPanel>, IRpcCallback
    {
        private static bool _hasLogin;
        private static string _email;
        private static string _userpassword;

        /// <summary>
        ///     服务器地址
        /// </summary>
        [LabelText("地址")] public string Address = "127.0.0.1";

        /// <summary>
        ///     服务器端口
        /// </summary>
        [LabelText("端口")] public uint Port = 7777;

        /// <summary>
        ///     登录提示文本
        /// </summary>
        [Header("")] [LabelText("登录提示")] public TMP_Text loginTip;

        /// <summary>
        ///     登录邮箱输入框
        /// </summary>
        [LabelText("登录邮箱")] public TMP_InputField loginEmail;

        /// <summary>
        ///     登录密码输入框
        /// </summary>
        [LabelText("登录密码")] public TMP_InputField loginUserpassword;

        /// <summary>
        ///     注册提示文本
        /// </summary>
        [LabelText("注册提示")] public TMP_Text registerTip;

        /// <summary>
        ///     注册邮箱输入框
        /// </summary>
        [LabelText("注册邮箱")] public TMP_InputField registerEmail;

        /// <summary>
        ///     注册用户名输入框
        /// </summary>
        [LabelText("注册用户名")] public TMP_InputField registerUsername;

        /// <summary>
        ///     注册密码输入框
        /// </summary>
        [LabelText("注册密码")] public TMP_InputField registerUserpassword;

        /// <summary>
        ///     注册验证码输入框
        /// </summary>
        [LabelText("注册验证码")] public TMP_InputField registerEmailcode;

        /// <summary>
        ///     登录面板游戏对象
        /// </summary>
        [Header("")] [LabelText("登录")] public GameObject LoginPanel;

        /// <summary>
        ///     注册面板游戏对象
        /// </summary>
        [LabelText("注册")] public GameObject RegisterPanel;

        /// <summary>
        ///     分组面板游戏对象
        /// </summary>
        [LabelText("组")] public GameObject GroupPanel;

        /// <summary>
        ///     好友预制体
        /// </summary>
        [Header("")] [LabelText("朋友")] public Friend FriendPrefab;

        /// <summary>
        ///     好友对象池的父物体
        /// </summary>
        [LabelText("朋友池")] public Transform FriendPool;

        /// <summary>
        ///     待处理好友预制体
        /// </summary>
        [Header("")] [LabelText("待处理好友预制件")] public PendingFriend PendingFriendPrefab;

        /// <summary>
        ///     待处理好友对象池的父物体
        /// </summary>
        [LabelText("待处理好友池")] public Transform PendingPool;

        /// <summary>
        ///     聊天面板组件
        /// </summary>
        [Header("")] [LabelText("聊天面板")] public ChatPanel ChatPanel;

        /// <summary>
        ///     输入好友名称的输入框
        /// </summary>
        [LabelText("好友名称")] public TMP_InputField FriendInput;

        /// <summary>
        ///     登录按钮
        /// </summary>
        [LabelText("登录按钮")] public Button LoginButton;

        /// <summary>
        ///     通讯录按钮
        /// </summary>
        [LabelText("通讯录按钮")] public Button TongXunLu;

        /// <summary>
        ///     注册按钮
        /// </summary>
        [LabelText("注册按钮")] public Button RegisterButton;

        /// <summary>
        ///     发送验证码按钮
        /// </summary>
        [LabelText("发送验证码按钮")] public Button SendEmailcodeButton;

        /// <summary>
        ///     发送好友请求按钮
        /// </summary>
        [LabelText("发送好友请求按钮")] public Button SendFriendButton;

        /// <summary>
        ///     通讯录面板的游戏对象
        /// </summary>
        [LabelText("通讯录面板")] public GameObject TongXunLuPanelGo;

        /// <summary>
        ///     聊天名称显示文本
        /// </summary>
        [LabelText("聊天名称")] public TMP_Text ChatName;

        /// <summary>
        ///     聊天面板组件
        /// </summary>
        [LabelText("聊天面板组件")] public ChatPanel chatPanel;

        /// <summary>
        ///     隐藏池1
        /// </summary>
        [LabelText("隐藏池1")] public Transform HidePool;

        /// <summary>
        ///     隐藏池2
        /// </summary>
        [LabelText("隐藏池2")] public Transform HidePool2;

        /// <summary>
        ///     在对象启动时调用的方法，初始化相关组件和事件监听器
        /// </summary>
        private void Start()
        {
            Screen.sleepTimeout = 600;
            chatPanel.OnAwake();
            NetworkTransport.Singleton.Master.RegisterHandlers(this, typeof(MainPanel));
            NetworkTransport.Singleton.Connect(IPAddress.Parse(Address), Port);
            LoginButton.onClick.AddListener(Login);
            TongXunLu.onClick.AddListener(GetFriends);
            TongXunLu.onClick.AddListener(GetPendingFriends);
            RegisterButton.onClick.AddListener(Register);
            SendEmailcodeButton.onClick.AddListener(SendEmailcode);
            SendFriendButton.onClick.AddListener(SendFriend);
            NetworkTransport.Singleton.Master.OnDisconnected += OnDisconnected;
            NetworkTransport.Singleton.Master.OnConnected += OnConnected;
        }

        /// <summary>
        ///     连接回调
        /// </summary>
        private void OnConnected()
        {
            if (_hasLogin)
                Send(new RequestLogin { email = _email, userpassword = _userpassword });
        }

        /// <summary>
        ///     断开回调
        /// </summary>
        private async void OnDisconnected()
        {
            if (Application.isPlaying)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(2f));
                NetworkTransport.Singleton.Connect(IPAddress.Parse(Address), Port);
            }
        }

        /// <summary>
        ///     在聊天面板中显示
        /// </summary>
        public void ShowChat()
        {
            TongXunLuPanelGo.SetActive(false);
            chatPanel.gameObject.SetActive(true);
        }

        /// <summary>
        ///     处理聊天消息的方法
        /// </summary>
        [Rpc]
        private void OnMessageChat(MessageChat message) => ChatPanel.Receive(message.username, message.content);

        /// <summary>
        ///     处理获取好友列表的方法
        /// </summary>
        [Rpc]
        private void OnGetFriends(ResponseGetFriends response)
        {
            var children = new Transform[FriendPool.childCount];
            for (var i = 0; i < FriendPool.childCount; ++i)
            {
                children[i] = FriendPool.GetChild(i);
            }

            for (var i = 0; i < children.Length; ++i)
            {
                children[i].SetParent(HidePool, false);
                var friend = children[i].GetComponent<Friend>();
                Entry.Pool.Push(friend);
            }

            for (var i = 0; i < response.friends.Count; ++i)
            {
                var friend = Entry.Pool.Pop<Friend>();
                if (friend != null)
                {
                    friend.transform.SetParent(FriendPool, false);
                }
                else
                {
                    friend = Instantiate(FriendPrefab.gameObject, FriendPool, false).GetComponent<Friend>();
                }

                friend.Name.text = response.friends[i];
            }
        }

        /// <summary>
        ///     处理获取待处理好友列表的方法
        /// </summary>
        [Rpc]
        private void OnGetPendingFriends(ResponseGetPendingFriends response)
        {
            var children = new Transform[PendingPool.childCount];
            for (var i = 0; i < PendingPool.childCount; ++i)
            {
                children[i] = PendingPool.GetChild(i);
            }

            for (var i = 0; i < children.Length; ++i)
            {
                children[i].SetParent(HidePool2, false);
                var t = children[i].GetComponent<PendingFriend>();
                Entry.Pool.Push(t);
            }

            for (var i = 0; i < response.friends.Count; ++i)
            {
                var friend = Entry.Pool.Pop<PendingFriend>();
                if (friend != null)
                {
                    friend.transform.SetParent(PendingPool, false);
                }
                else
                {
                    friend = Instantiate(PendingFriendPrefab.gameObject, PendingPool, false).GetComponent<PendingFriend>();
                }

                friend.Name.text = response.friends[i];
            }
        }

        /// <summary>
        ///     处理登录响应的方法
        /// </summary>
        [Rpc]
        private void OnResponseLogin(ResponseLogin response)
        {
            loginTip.text = response.Message;
            _hasLogin = response.Success;
            if (response.Success)
            {
                LoginPanel.SetActive(false);
                RegisterPanel.SetActive(false);
                GroupPanel.SetActive(true);
            }
        }

        /// <summary>
        ///     获取好友列表的方法
        /// </summary>
        [BindButton(nameof(TongXunLu))]
        public void GetFriends() => Send(new RequestGetFriends());

        /// <summary>
        ///     获取待处理好友列表的方法
        /// </summary>
        [BindButton(nameof(TongXunLu))]
        public void GetPendingFriends() => Send(new RequestGetPendingFriends());

        /// <summary>
        ///     发送好友请求的方法
        /// </summary>
        [BindButton(nameof(SendFriendButton))]
        public void SendFriend()
        {
            var username = FriendInput.text;
            if (string.IsNullOrEmpty(username))
                return;
            Send(new RequestFriend { username = username });
        }

        /// <summary>
        ///     处理注册响应的方法
        /// </summary>
        [Rpc]
        private void OnResponseRegister(ResponseRegister response) => registerTip.text = response.Message;

        /// <summary>
        ///     发送网络消息的通用方法
        /// </summary>
        private void Send<T>(T value) where T : struct, INetworkMessage, IMemoryPackable<T> => NetworkTransport.Singleton.Master.Send(value);

        /// <summary>
        ///     处理登录的方法
        /// </summary>
        [BindButton(nameof(LoginButton))]
        public void Login()
        {
            var email = loginEmail.text;
            var userpassword = loginUserpassword.text;
            _email = loginEmail.text;
            _userpassword = loginUserpassword.text;
            Send(new RequestLogin { email = email, userpassword = userpassword });
        }

        /// <summary>
        ///     处理注册的方法
        /// </summary>
        [BindButton(nameof(RegisterButton))]
        public void Register()
        {
            var email = registerEmail.text;
            var username = registerUsername.text;
            var userpassword = registerUserpassword.text;
            var emailcode = registerEmailcode.text;
            Send(new RequestRegister { email = email, username = username, userpassword = userpassword, emailcode = emailcode });
        }

        /// <summary>
        ///     发送验证码的方法
        /// </summary>
        [BindButton(nameof(SendEmailcodeButton))]
        public void SendEmailcode()
        {
            var email = registerEmail.text;
            Send(new RequestSendCode { email = email });
        }
    }
}