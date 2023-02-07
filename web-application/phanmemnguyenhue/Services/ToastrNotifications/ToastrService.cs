using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;

namespace phanmemnguyenhue.Services.ToastrNotifications
{
    public class ToastrService : IEnumerable<Toastr>
    {
        private readonly TempDataDictionary _tempData;
        private readonly List<Toastr> _messageList;
        private readonly string _key = typeof(Toastr).FullName;

        public ToastrService(TempDataDictionary tempData)
        {
            _messageList = new List<Toastr>();
            if (tempData != null)
            {
                _tempData = tempData;
                var currentMessages = _tempData[_key] as IEnumerable<Toastr> ?? new List<Toastr>();
                _messageList.AddRange(currentMessages);
            }
        }

        public int Count
        {
            get
            {
                return _messageList.Count;
            }
        }

        public void Add(Toastr item)
        {
            _messageList.Add(item);
            SaveMessageList();
        }

        private void SaveMessageList()
        {
            _tempData[_key] = _messageList;
            _tempData.Keep(_key);
        }

        public IEnumerator<Toastr> GetEnumerator()
        {
            try
            {
                return _messageList.GetEnumerator();
            }
            finally
            {
                _tempData.Remove(_key);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}