using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSLua
{
    class HonorEventItem
    {
        public long PlayerID;
        public int heroID;
        public int honorID;
        public int multiKillNum;
    }


    class HonorEventView
    {

        MutliImageQueue<HonorEventItem> mMultiKillQueue;


        protected void OnShow()
        {
            mMultiKillQueue = new MutliImageQueue<HonorEventItem>(OnStartShow, 0f, 4.0f, 1.0f);
        }
        void OnStartShow(HonorEventItem item)
        {
        }
  }
}
