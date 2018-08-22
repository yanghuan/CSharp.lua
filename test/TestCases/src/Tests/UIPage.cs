namespace CSLua
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public enum UIType
    {
        Normal,
        Fixed,
        PopUp,
        None,      //独立的窗口
    }

    public enum UIMode
    {
        DoNothing,
        HideOther,     // 闭其他界面
        NeedBack,      // 点击返回按钮关闭当前,不关闭其他界面(需要调整好层级关系)
        NoNeedBack,    // 关闭TopBar,关闭其他界面,不加入backSequence队列
    }

    public enum UICollider
    {
        None,      // 显示该界面不包含碰撞背景
        Normal,    // 碰撞透明背景
        WithBg,    // 碰撞非透明背景
    }

    public abstract class UIPage : Object
    {
        public string name = string.Empty;
        public int id = -1;
        public UIType type = UIType.Normal;
        public UIMode mode = UIMode.DoNothing;


        //all pages with the union type
        private static Dictionary<string, UIPage> m_allPages = new Dictionary<string, UIPage>();
        //control 1>2>3>4>5 each page close will back show the previus page.
        private static List<UIPage> m_currentPageNodes = new List<UIPage>();

        public UIPage() { }

        public UIPage(UIType type, UIMode mod, UICollider col)
        {
            this.type = type;
            this.mode = mod;
        }

    }

    public abstract class UIPage<T> : UIPage
    {
        public readonly int controls = 0;
        public UIPage()
        {
        }

        public UIPage(UIType type, UIMode mod, UICollider col)
            : base(type, mod, col)
        {

        }
    }


    public class NewUILoginPage : UIPage<string>
    {
        public NewUILoginPage() : base(UIType.Normal, UIMode.HideOther, UICollider.None)
        {
        }
    }


    public class NewUILoginTest
    {
        [TestCase]
        public static void Test()
        {
            new NewUILoginPage();
        }
    };

}