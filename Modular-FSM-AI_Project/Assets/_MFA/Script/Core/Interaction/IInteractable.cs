namespace MFA.Core.Interaction
{
    /// <summary>
    /// Contrato para todos los objetos del juego que acepten interacciones y enfoques.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>Invocado cuando la caja de detección entra en contacto/enfoque con el objeto.</summary>
        void OnFocusGained();

        /// <summary>Invocado cuando la caja de detección pierde el contacto/enfoque con el objeto.</summary>
        void OnFocusLost();

        /// <summary>Invocado al presionar la acción de interacción.</summary>
        void OnInteractPressed();

        /// <summary>Invocado al soltar la acción de interacción.</summary>
        void OnInteractReleased();
    }
}