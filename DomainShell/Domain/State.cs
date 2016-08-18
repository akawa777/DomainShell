using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace DomainShell.Domain
{
    public class State
    {
        public enum StateFlg
        {
            UnChanged = 0,
            New = 1,
            Modified = 2,
            Deleted = 3
        }

        private StateFlg _stateFlg = StateFlg.UnChanged;

        public StateFlg GetState()
        {
            return _stateFlg;
        }

        public void New()
        {
            if (_stateFlg == StateFlg.UnChanged)
            {
                _stateFlg = StateFlg.New;
            }
        }

        public void Modified()
        {
            if (_stateFlg != StateFlg.New)
            {
                _stateFlg = StateFlg.Modified;
            }
        }

        public void Deleted()
        {
            _stateFlg = StateFlg.Deleted;
        }

        public void UnChanged()
        {
            _stateFlg = StateFlg.UnChanged;
        }
    }
}
