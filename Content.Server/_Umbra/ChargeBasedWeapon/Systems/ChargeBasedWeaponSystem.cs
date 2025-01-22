using Content.Shared._Umbra.ChargeBasedWeapon;
using Content.Server.Power.Components;
using Content.Shared.Examine;
using Content.Shared.Item;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared._Umbra.ChargeBasedWeapon;
using Content.Server.Power.EntitySystems;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee.Events;
using Content.Server._Umbra.ChargeBasedWeapon.Components;

namespace Content.Server._Umbra.ChargeBasedWeapon.Systems;

public sealed class ChargeBasedWeaponSystem : SharedChargeBasedWeaponSystem
{
    [Dependency] private readonly SharedItemSystem _item = default!;
    [Dependency] private readonly ItemToggleSystem _itemToggle = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly BatterySystem _battery = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ChargeBasedWeaponComponent, ExaminedEvent>(OnExamined);
        SubscribeLocalEvent<ChargeBasedWeaponComponent, AttemptMeleeEvent>(OnHitAttempt);
        SubscribeLocalEvent<ChargeBasedWeaponComponent, ItemToggleActivateAttemptEvent>(TryTurnOn);
        SubscribeLocalEvent<ChargeBasedWeaponComponent, ItemToggledEvent>(ToggleDone);
        SubscribeLocalEvent<ChargeBasedWeaponComponent, ChargeChangedEvent>(OnChargeChanged);
    }

    private void OnHitAttempt(Entity<ChargeBasedWeaponComponent> entity, ref AttemptMeleeEvent args)
    {
        if (!_itemToggle.IsActivated(entity.Owner) ||
        !TryComp<BatteryComponent>(entity.Owner, out var battery) || !_battery.TryUseCharge(entity.Owner, entity.Comp.EnergyPerUse, battery))
        {
            args.Cancelled = true;
        }
    }

    private void OnExamined(Entity<ChargeBasedWeaponComponent> entity, ref ExaminedEvent args)
    {
        var onMsg = _itemToggle.IsActivated(entity.Owner)
        ? Loc.GetString("comp-stunbaton-examined-on")
        : Loc.GetString("comp-stunbaton-examined-off");
        args.PushMarkup(onMsg);

        if (TryComp<BatteryComponent>(entity.Owner, out var battery))
        {
            var count = (int)(battery.CurrentCharge / entity.Comp.EnergyPerUse);
            args.PushMarkup(Loc.GetString("melee-battery-examine", ("color", "yellow"), ("count", count)));
        }
    }

    private void ToggleDone(Entity<ChargeBasedWeaponComponent> entity, ref ItemToggledEvent args)
    {
        _item.SetHeldPrefix(entity.Owner, args.Activated ? "on" : "off");
    }

    private void TryTurnOn(Entity<ChargeBasedWeaponComponent> entity, ref ItemToggleActivateAttemptEvent args)
    {
        if (!TryComp<BatteryComponent>(entity, out var battery) || battery.CurrentCharge < entity.Comp.EnergyPerUse)
        {
            args.Cancelled = true;
            if (args.User != null)
            {
                _popup.PopupEntity(Loc.GetString("stunbaton-component-low-charge"), (EntityUid)args.User, (EntityUid)args.User);
            }
            return;
        }
    }

    private void OnChargeChanged(Entity<ChargeBasedWeaponComponent> entity, ref ChargeChangedEvent args)
    {
        if (TryComp<BatteryComponent>(entity.Owner, out var battery) &&
            battery.CurrentCharge < entity.Comp.EnergyPerUse)
        {
            _itemToggle.TryDeactivate(entity.Owner, predicted: false);
        }
    }
}
