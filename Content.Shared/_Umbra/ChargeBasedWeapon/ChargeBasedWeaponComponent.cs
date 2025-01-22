using Content.Shared._Umbra.ChargeBasedWeapon;
using Robust.Shared.GameStates;

namespace Content.Server._Umbra.ChargeBasedWeapon.Components;

[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
[Access(typeof(SharedChargeBasedWeaponSystem))]
public sealed partial class ChargeBasedWeaponComponent : Component
{
    [DataField("energyPerUse"), ViewVariables(VVAccess.ReadWrite)]
    [AutoNetworkedField]
    public float EnergyPerUse = 350;
}
