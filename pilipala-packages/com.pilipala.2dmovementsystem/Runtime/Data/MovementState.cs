namespace Pilipala.Movement.Data
{
    public enum MovementState
    {
        Normal,     // default state, can move and shoot
        Shooting,   // while firing weapon
        Prone,      // while crouching/prone
        Jumping,    // while jumping
        Invincible, // after taking damage
        Dead        // when health reaches 0
    }
}