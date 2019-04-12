using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Study.SignalRdemo
{
    public class MessageAppService
    {
        private static IDictionary<string, RealOnlineClient> onlines = new Dictionary<string, RealOnlineClient>();

        /// <summary>
        /// 添加在线用户
        /// </summary>
        /// <param name="connctionId"></param>
        /// <param name="client"></param>
        public void AddClient(string connctionId, RealOnlineClient client)
        {
            onlines.Add(connctionId, client);
        }
        /// <summary>
        /// 判断数据中是否已经存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public RealOnlineClient IsOnline(string name)
        {
            return onlines.Where(o => o.Value.IdentityName == name).FirstOrDefault().Value;
        }
        /// <summary>
        /// 修改，由于刷新存在连接ID变化
        /// </summary>
        /// <param name="connctionId"></param>
        /// <param name="client"></param>
        public void UpdateClient(string connctionId, RealOnlineClient client)
        {
            onlines[connctionId] = client;
        }
        /// <summary>
        /// 移除下线用户
        /// </summary>
        /// <param name="connctionId"></param>
        public void RemoveClient(string connctionId)
        {
            if (onlines.Keys.Contains(connctionId))
                onlines.Remove(connctionId);
        }

        public IDictionary<string, RealOnlineClient> GetClients()
        {
            return onlines;
        }

        public int ClientsCount()
        {
            return onlines.Count;
        }
    }
}
