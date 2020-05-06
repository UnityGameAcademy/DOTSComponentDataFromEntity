using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class FaceDirectionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.
            WithAll<PlayerTag>().
            ForEach((ref Rotation rot, in Translation pos, in MoveData moveData) =>
            {
                FaceDirection(ref rot, moveData);

            }).Schedule();

        Entities.
            WithNone<PlayerTag>().
            WithAll<ChaserTag>().
            ForEach((ref MoveData moveData, ref Rotation rot, in Translation pos, in TargetData targetData) =>
            {
                ComponentDataFromEntity<Translation> allTranslations = GetComponentDataFromEntity<Translation>(true);
                Translation targetPos = allTranslations[targetData.targetEntity];

                float3 dirToTarget = targetPos.Value - pos.Value;
                moveData.direction = dirToTarget;
                FaceDirection(ref rot, moveData);

            }).Run();

    }

    private static void FaceDirection(ref Rotation rot, MoveData moveData)
    {
        if (!moveData.direction.Equals(float3.zero))
        {
            quaternion targetRotation = quaternion.LookRotationSafe(moveData.direction, math.up());
            rot.Value = math.slerp(rot.Value, targetRotation, moveData.turnSpeed);
        }
    }
}
