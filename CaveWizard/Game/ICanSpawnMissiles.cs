
using CaveEngine.WorldSystem;

namespace CaveWizard.Game
{
    public interface ICanSpawnMissiles
    {
        void RemoveMissiles(WorldObject missile);

        void AddToRemoveLater(WorldObject missile);
    }
}