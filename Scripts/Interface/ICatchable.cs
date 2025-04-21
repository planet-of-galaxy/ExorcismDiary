using UnityEngine;
public interface ICatchable
{
    bool IsOutOfControl { get; } // 是否已经失控
    void OutOfControl(bool isOutOfControl); // 让被捕捉的对象失控
    Transform Catched(); // 把位置交给猎人处理
}
