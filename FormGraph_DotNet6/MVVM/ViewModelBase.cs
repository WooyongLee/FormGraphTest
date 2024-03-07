using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormGraph_DotNet6
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged 구현부

        public event PropertyChangedEventHandler PropertyChanged;

        // 각 Property 이름으로 지정해 놓고 UI 쪽으로 변경에 대한 이벤트 구현
        protected void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged 구현부
    }
}
