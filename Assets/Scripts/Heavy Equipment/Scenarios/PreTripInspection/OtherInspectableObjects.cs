using RemoteEducation.Scenarios;
using System.Collections.Generic;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment
{
    public class OtherInspectableObjects : MonoBehaviour, ITaskable
    {
        public List<Pin> Stage8PinsAndRetainers;
        public List<Pin> BackhoeRightPins; //Stage 9
        public List<Pin> BackhoeLeftPins; //Stage 10
        public List<Pin> Stage14SwingLinkagePins; //Stage 14
        public List<Pin> Stage15RearBucketPins; //Stage 15-19

        public List<Pin> Stage13LeftGrouserPins; //Stage 131
        public List<Pin> Stage13RightGrouserPins; //Stage 132

        public bool Cleared = false;
        public bool Stage13LeftGrouserPinsCleared = false;
        public bool Stage13RightGrouserPinsCleared = false;
        public bool RightSidePinsCleared = false; //Stage 9
        public bool LeftSidePinsCleared = false; //Stage 10
        public bool Stage14SwingLinkagePinsCleared = false; //Stage 14
        public bool Stage15RearBucketPinsCleared = false; //Stage 15-19

        public TaskableObject Taskable { get; private set; }

        /// <summary>Initialize each individual debris in the group (cluster).</summary>
        /// <param name="input"><see cref="BackhoeController"/> instance argument.</param>
        private void Start()
        {
            Taskable = new TaskableObject(this);

            foreach (Pin pin in Stage8PinsAndRetainers) { pin.AttachToInspector(this); }
            foreach (Pin pin in BackhoeLeftPins) { pin.AttachToInspector(this); }
            foreach (Pin pin in BackhoeRightPins) { pin.AttachToInspector(this); }
            foreach (Pin pin in Stage14SwingLinkagePins) { pin.AttachToInspector(this); }
            foreach (Pin pin in Stage15RearBucketPins) { pin.AttachToInspector(this); }
            foreach (Pin pin in Stage13LeftGrouserPins) { pin.AttachToInspector(this); }
            foreach (Pin pin in Stage13RightGrouserPins) { pin.AttachToInspector(this); }
        }

        public void Poke(int stage)
        {
            if (stage == 8)
            {
                Debug.Log("checking stage 8");
                foreach (Pin pin in Stage8PinsAndRetainers)
                {
                    if (!pin.CheckTask())
                    {
                        return;
                    }
                }
                ResetAllStates();
                Cleared = true;
            }

            else if (stage == 9)
            {
                Debug.Log("checking stage 9");
                foreach (Pin pin in BackhoeLeftPins)
                {
                    if (!pin.CheckTask())
                    {
                        return;
                    }
                }
                ResetAllStates();
                LeftSidePinsCleared = true;
            }

            else if (stage == 10)
            {
                Debug.Log("checking stage 10");
                foreach (Pin pin in BackhoeRightPins)
                {
                    if (!pin.CheckTask())
                    {
                        return;
                    }
                }
                ResetAllStates();
                RightSidePinsCleared = true;
            }

            else if (stage == 131)
            {
                Debug.Log("checking stage 131");
                foreach (Pin pin in Stage13LeftGrouserPins)
                {
                    if (!pin.CheckTask())
                    {
                        return;
                    }
                }
                ResetAllStates();
                Stage13LeftGrouserPinsCleared = true;
            }

            else if (stage == 132)
            {
                Debug.Log("checking stage 132");
                foreach (Pin pin in Stage13RightGrouserPins)
                {
                    if (!pin.CheckTask())
                    {
                        return;
                    }
                }
                ResetAllStates();
                Stage13RightGrouserPinsCleared = true;
            }

            else if (stage == 14)
            {
                Debug.Log("checking stage 14");
                foreach (Pin pin in Stage14SwingLinkagePins)
                {
                    if (!pin.CheckTask())
                    {
                        return;
                    }
                }
                ResetAllStates();
                Stage14SwingLinkagePinsCleared = true;
            }

            else if (stage == 15)
            {
                Debug.Log("checking stage 15-19");
                foreach (Pin pin in Stage15RearBucketPins)
                {
                    if (!pin.CheckTask())
                    {
                        return;
                    }
                }
                ResetAllStates();
                Stage15RearBucketPinsCleared = true;
            }

            Taskable.PokeTaskManager();
        }

        void ResetAllStates() {
            Cleared = false;
            Stage13LeftGrouserPinsCleared = false;
            Stage13RightGrouserPinsCleared = false;
            RightSidePinsCleared = false;
            LeftSidePinsCleared = false;
            Stage14SwingLinkagePinsCleared = false;
            Stage15RearBucketPinsCleared = false;
        }

        public object CheckTask(int checkType, object inputData)
        {
            switch (checkType)
            {
                case (int)TaskVertexManager.CheckTypes.Int:
                    int clearInt =
                        Stage15RearBucketPinsCleared ? 5 :
                        Stage14SwingLinkagePinsCleared ? 4 :
                        Stage13RightGrouserPinsCleared ? 6 :
                        Stage13LeftGrouserPinsCleared ? 7 :
                        RightSidePinsCleared ? 2 :
                        LeftSidePinsCleared ? 3 :
                        Cleared ? 1 : 0;

                    Debug.Log(clearInt);
                    return clearInt;

                default:
                    Debug.LogError($"Invalid check type passed into {GetType().Name}");
                    break;
            }

            return null;
        }
    }
}