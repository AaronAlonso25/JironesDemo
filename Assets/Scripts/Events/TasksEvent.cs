using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace Caserta
{
    public class TasksEvent : MonoBehaviour
    {
        public List<string> Tasks;

        [Header("Events")]
        public UnityEvent OnEnd;

        [Header("Debug")]
        public List<string> currentTasks;

        void Start()
        {

        }

        public void AddTask(string nameTask)
        {
            if (!this.currentTasks.Contains(nameTask))
                this.currentTasks.Add(nameTask);

            this.CheckEndTask();
        }

        private void CheckEndTask()
        {
            if (this.Tasks.Count == this.currentTasks.Count)
            {
                this.OnEnd.Invoke();
            }                
        }

    }
}