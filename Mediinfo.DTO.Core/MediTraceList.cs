using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Mediinfo.DTO.Core
{
    /// <summary>
    /// 跟踪对象状态的BindingList
    /// 状态变化:
    /// 新增操作: 对象的State为New
    /// 删除操作：如果对象的State是New则直接抛弃；否则则放到删除列表中，并将对象状态修改为Delete
    /// 修改操作: 如果对象原来的状态为New,则不变；如果是UnChange则变为Update
    /// </summary>
    /// <typeparam name="T">DTO对象</typeparam>
    [Serializable]
    public class MediTraceList<T> : MediBindingList<T> where T : DTOBase
    {
        private List<T> _deleteList;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MediTraceList() : base()
        {
            _deleteList = new List<T>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="list"></param>
        public MediTraceList(IList<T> list) : base(list)
        {
            _deleteList = new List<T>();
            foreach (var item in list)
            {
                // list加入到meditracelist中后设置为可跟踪
                item.SetTraceChange(true);
            }
        }

        /// <summary>
        /// 重置List,重置后原有List的状态及数据将消失
        /// </summary>
        /// <param name="list"></param>
        public void SetList(IList<T> list)
        {
            _deleteList.Clear();
            this.Clear();

            DTOState oldState;

            foreach (var item in list)
            {
                // list加入到meditracelist中后设置为可跟踪
                item.SetTraceChange(true);

                oldState = item.GetState();
                this.Add(item);
                item.SetState(oldState);
            }
        }

        /// <summary>
        /// 在 _deleteList中添加实体，非特殊业务需求请谨慎使用
        /// add by songxl on 2019-03-15
        /// </summary>
        /// <param name="t"></param>
        public void AddEntityToDeleteList(T t)
        {
            _deleteList.Add(t);
        }

        /// <summary>
        /// 两个MediTraceList相加
        /// add by songxl on 2019-03-15
        /// </summary>
        /// <param name="ts"></param>
        public void MediTraceListPlus(MediTraceList<T> ts)
        {
            foreach (var o in ts)
            {
                var state = o.GetState();   // 获取原来的状态
                this.Add(o);
                o.SetState(state);      // 对于leftList来说item的状态是New,需要把他变回原来的状态
            }

            // 再把_deleteList中的内容相加
            var changeList = ts.GetChanges();
            foreach (var p in changeList)
            {
                if (p.GetState() == DTOState.Delete)
                {
                    _deleteList.Add(p);
                }
            }
        }

        /// <summary>
        /// 重写列表变化事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnListChanged(ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemChanged)
            {
                // 添加以后需要强制设置为状态跟踪
                this[e.NewIndex].SetTraceChange(true);

                DTOBase changeItem = this[e.NewIndex];

                // 将对象的UnChange状态修改为Update状态
                if (changeItem.GetState() == DTOState.UnChange)
                    changeItem.SetState(DTOState.Update);
            }
            else if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                // 添加以后需要强制设置为状态跟踪
                this[e.NewIndex].SetTraceChange(true);

                this[e.NewIndex].SetState(DTOState.New);
            }
            base.OnListChanged(e);
        }

        /// <summary>
        /// 移除对象
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            var item = this[index];
            if (item.GetState() != DTOState.New)
            {
                item.SetState(DTOState.Delete);
                _deleteList.Add(item);
            }

            base.RemoveItem(index);
        }

        /// <summary>
        /// 判断列表是否被修改过
        /// </summary>
        /// <returns>true:被修改过，false:没有被修改</returns>
        public bool HasChanged()
        {
            if (_deleteList.Count > 0)
                return true;

            var changeItem = this.Where(c => c.GetState() == DTOState.New || c.GetState() == DTOState.Update).FirstOrDefault();
            if (null != changeItem)
                return true;

            return false;
        }

        /// <summary>
        /// 将MediTraceList中的对象重置为未修改状态，同时清空删除对象列表
        /// </summary>
        public void ResetChangeStatus()
        {
            _deleteList.Clear();

            foreach (var item in this)
            {
                item.SetState(DTOState.UnChange);
            }
        }

        /// <summary>
        /// 取得变化的数据，包括（新增New、修改Update、删除Delete）
        /// </summary>
        /// <returns></returns>
        public List<T> GetChanges()
        {
            List<T> changeList = new List<T>();

            changeList.AddRange(_deleteList);
            changeList.AddRange(this.Where(c => c.GetState() == DTOState.New).ToList());
            changeList.AddRange(this.Where(c => c.GetState() == DTOState.Update).ToList());

            return changeList;
        }
    }
}
