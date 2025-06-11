using RemoteEducation.Interactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemoteEducation.Interactions.Tests
{
    /// <summary>
    /// Used in unit testing.
    /// </summary>
    public class TestSelectable : Selectable
    {
        public int ATestInt;
        public bool ATestBool;

        private ExternalIsEnabled externalIsEnabled;

        public override bool IsEnabled
        {
            get
            {
                if (!base.IsEnabled)
                {
                    return false;
                }

                if (externalIsEnabled != null)
                {
                    return externalIsEnabled.Invoke();
                }

                return true;

            }
        }

        public void SetExternalIsEnabled(ExternalIsEnabled externalIsEnabled)
        {
            this.externalIsEnabled = externalIsEnabled;
        }
    }
}