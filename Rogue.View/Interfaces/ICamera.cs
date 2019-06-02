﻿namespace Rogue.View.Interfaces
{
    using Rogue.Types;

    public interface ICamera
    {
        void MoveCamera(Direction direction, bool stop = false);

        void SetCamera(double x, double y);

        void ResetCamera();

        double CameraOffsetX { get; }

        double CameraOffsetY { get; }

        double CameraOffsetLimitX { get; }

        double CameraOffsetLimitY { get; }

        void SetCameraSpeed(double speed);
    }
}
