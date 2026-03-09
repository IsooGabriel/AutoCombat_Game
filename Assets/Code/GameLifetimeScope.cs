using VContainer;
using VContainer.Unity;
using Weapon;

public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<Character>();
    }
}
