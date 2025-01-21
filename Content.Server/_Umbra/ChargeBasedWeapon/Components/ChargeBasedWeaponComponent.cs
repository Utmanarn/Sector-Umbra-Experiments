using Robust.Shared.GameStates;

namespace Content.Server._Umbra.ChargeBasedWeapon.Components;

[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class ChargeBasedWeaponComponent
{
    [DataField("energyPerUse"), ViewVariables(VVAccess.ReadWrite)]
    [AutoNetworkedField]
    public float EnergyPerUse = 350;
}
