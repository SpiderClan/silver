using Content.Shared._Silver.AdaptiveCoordinateDiskBox;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction.Events;
using Content.Shared.Shuttles.Components;
using Robust.Shared.Prototypes;

namespace Content.Server._Silver.AdaptiveCoordinateDiskBox;

/// <summary>
/// This handles...
/// </summary>

public sealed class AdaptiveCoordinateDiskBoxSystem : EntitySystem
{
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly IEntityManager _entMan = default!;
    [Dependency] private readonly SharedHandsSystem _hands = default!;

    public static readonly EntProtoId CoordinatesDisk = "CoordinatesDisk";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AdaptiveCoordinateDiskBoxComponent, UseInHandEvent>(OnUse);
    }

    private void OnUse(EntityUid uid, AdaptiveCoordinateDiskBoxComponent component, ref UseInHandEvent args)
    {
        if (args.Handled)
            return;

        var user = args.User;
        var coords = _entMan.GetComponent<TransformComponent>(user).Coordinates;
        var map = _transform.GetMap(user);
        EntityUid cdUid = _entMan.SpawnEntity(CoordinatesDisk, coords);
        var cd = _entMan.EnsureComponent<ShuttleDestinationCoordinatesComponent>(cdUid);
        cd.Destination = map;

        QueueDel(uid);
        _hands.TryPickup(user, cdUid);
        args.Handled = true;
    }
}
