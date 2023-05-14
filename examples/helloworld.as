PlayerController :: Class {
    transform: Transform
    br: BoatRenderer
    gold: Int
    main: MainScene

    Awake :: () -> void {
        transform = Get(Transform)
        br = Get(BoatRenderer)
        main = SceneHandler::Get(MainScene)
    }

    Sleep :: () -> void {

    }

    Update :: () -> void {
        Debug::RegisterCommand(command: "getpos", callback: () -> void {
            Debug::Log(message: transform.Position)
        })

        mSpeed: float = 70

        if Keyboard.Down(::A) {
            transform.Angle -= 100f * DeltaTime
        }

        if Keyboard.Down(::D) {
            transform.Angle += 100f * DeltaTime 
        }

        if Keyboard.Down(::W) {
            transform.Position += Helpers::LengthDir(length: mSpeed * DeltaTime, angle: transform.Angle)
        
            if Math::NextFloat() > 0.98f {
                br.v.Add(Vector3 {
                    x: transform.Position,
                    y: transform.Position,
                    z: 1
                })
            }
        }

        if Mouse.Pressed(::Left) {
            Get(BoatShooterController).Shoot()
        }

        if transform.Position.x < -3200 || transform.Position.x > 3200 || transform.Position.y < -3200 | transform.Position.y > 3200 {
            SceneHandler::Unload("Main")
            SceneHandler::Load("LostAtSea")
        }

        for j: 0..main.Map.Count {
            foreach i: main.Islands[main.Map[j].0] {
                if Helpers::PointInside(point: transform.Position, rect: Vector4 {
                    x: i.Key.X * 16 - main.Map[j].1.x,
                    y: i.Key.y * 16 - main.Map[j].1.y, 
                    z: 16,
                    w: 16
                }) {
                    transform.Position += Helpers::LengthDir(length: mSpeed * DeltaTime, angle: transform.Angle + 180)
                }
            }
        }
    }
} 