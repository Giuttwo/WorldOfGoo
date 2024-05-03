using Microsoft.Xna.Framework;

public class Camera
{
    private Vector2 position;
    private float zoom;

    // Propiedad para obtener la posición actual de la cámara.
    public Vector2 Position
    {
        get { return position; }
        private set { position = value; UpdateTransform(); }
    }

    // Propiedad para obtener y establecer el zoom de la cámara.
    public float Zoom
    {
        get { return zoom; }
        set { zoom = value; UpdateTransform(); }
    }

    // Matriz de transformación que se aplica en el SpriteBatch.Begin
    public Matrix Transform { get; private set; }

    // Matriz de transformación inversa para transformaciones de coordenadas
    public Matrix InverseTransform { get; private set; }

    public Camera()
    {
        position = Vector2.Zero;
        zoom = 1.0f; // Un zoom de 1 significa sin zoom
        UpdateTransform();
    }

    public void Reset() {
        position = Vector2.Zero;
        zoom = 1.0f;

    }

    // Método para mover la cámara por una cantidad dada
    public void Move(Vector2 amount)
    {
        Position += amount;
    }

    // Método para actualizar la matriz de transformación
    private void UpdateTransform()
    {
        Transform = Matrix.CreateTranslation(new Vector3(-position, 0.0f)) *
                    Matrix.CreateScale(zoom, zoom, 1.0f);

        InverseTransform = Matrix.Invert(Transform);
    }
}
