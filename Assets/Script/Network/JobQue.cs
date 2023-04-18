using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NetworkCore
{
    public interface IJobQueue
    {
        void Push(Action job);
    }


    public class JobQue : IJobQueue
    {
        Queue<Action> _jobQue = new Queue<Action>();
        object _lock = new object();
        bool _flush = false;

        public void Push(Action job)
        {
            bool flush = false;

            lock (_lock)
            {
                //���� ��������Ʈ�� _jobQueue�� �ִ´�.
                _jobQue.Enqueue(job);
                //ť�� ���� ������� false�� ��ȯ
                if (_flush == false)
                    //���µ����ϰ�
                    flush = _flush = true;
            }

            if (flush)
                Flush();
        }

        void Flush()
        {
            //����, �� ����
            while (true)
            {
                Action action = Pop();
                if (action == null)
                    return;

                action.Invoke();
            }
        }

        Action Pop()
        {
            lock (_lock)
            {
                if (_jobQue.Count == 0)
                {
                    //���� ������ ��ȯ
                    _flush = false;
                    return null;
                }
                return _jobQue.Dequeue();
            }
        }
    }

}