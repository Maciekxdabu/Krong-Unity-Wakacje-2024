
namespace Assets.Scripts.Runtime.Character
{
    public class Enemy : Creature
    {
        public void TakeDamage(float value)
        {
            _hp -= value;

            UnityEngine.Debug.Log("_hp "+ _hp);
        }
    }
}