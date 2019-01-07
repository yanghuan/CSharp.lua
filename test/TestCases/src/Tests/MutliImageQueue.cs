using System;
using System.Collections.Generic;

namespace CSLua
{
    class MutliImageQueue<T>
    {
        public enum ImageRunState
        {
            None,
            ToDisplay,
            Displayed,
            ToHide,
        }

        class MutliIShowInfo
        {
            public T item;
            public bool isShowImmediately;

            public MutliIShowInfo(T item, bool isShow)
            {
                this.item = item;
                this.isShowImmediately = isShow;
            }
        }

        List<MutliIShowInfo> mutliShowList = new List<MutliIShowInfo>();

        private float showTime;
        private float hideTime;
        private float durTime;

        private float startShowTime;

        Action<T> OnShowCall;

        private ImageRunState runState = ImageRunState.None;
        public MutliImageQueue( Action<T> call, float statrTime = 0, float dur = 0, float endTime = 0)
        {
            showTime = statrTime;
            durTime = dur;
            hideTime = endTime;
            OnShowCall = call;
        }


        public void AddToMultiQueue(T item, bool isShowImmediately = false)
        {
            MutliIShowInfo showInfo = new MutliIShowInfo(item, isShowImmediately);
            mutliShowList.Add(showInfo);
        }

    }
}
