using UnityEngine;

namespace Pilipala.Movement.Interfaces
{
    public interface IAimInputProvider
    {
        Vector2 CurrentRawAimInput { get; }
    }
}