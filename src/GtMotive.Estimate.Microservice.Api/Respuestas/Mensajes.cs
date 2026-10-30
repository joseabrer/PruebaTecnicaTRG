namespace GtMotive.Estimate.Microservice.Api.Respuestas
{
    /// <summary>
    /// Todos los textos que la API envía a sus clientes, en un solo sitio: títulos y detalles de las respuestas de error
    /// y mensajes de validación. Los controllers, presenters, filtros y clases de solicitud los reutilizan, así que un texto
    /// se escribe una sola vez y un error se lee siempre igual, venga de donde venga.
    /// </summary>
    public static class Mensajes
    {
        /// <summary>
        /// Título de una respuesta 400, por una regla o una validación que la petición no cumple.
        /// </summary>
        public const string TituloSolicitudInvalida = "Solicitud no válida";

        /// <summary>
        /// Título de una respuesta 404.
        /// </summary>
        public const string TituloNoEncontrado = "No encontrado";

        /// <summary>
        /// Título de una respuesta 409.
        /// </summary>
        public const string TituloConflicto = "Conflicto";

        /// <summary>
        /// Título de una respuesta 500.
        /// </summary>
        public const string TituloErrorInterno = "Error interno del servidor";

        /// <summary>
        /// Detalle de una respuesta 500. El mensaje real solo se escribe en el log: puede revelar detalles internos.
        /// </summary>
        public const string DetalleErrorInterno = "Ha ocurrido un error inesperado. Se ha registrado en el log.";

        /// <summary>
        /// Detalle de una respuesta 400 causada por campos inválidos.
        /// </summary>
        public const string DetalleSolicitudInvalida = "Revisa los campos indicados y vuelve a intentarlo.";

        /// <summary>
        /// Mensaje de validación de un campo obligatorio. {0} es el nombre del campo.
        /// </summary>
        public const string Obligatorio = "El campo {0} es obligatorio.";

        /// <summary>
        /// Mensaje de validación de un texto demasiado largo. {0} es el nombre del campo y {1} la longitud máxima.
        /// </summary>
        public const string LongitudMaxima = "El campo {0} no puede superar los {1} caracteres.";

        /// <summary>
        /// Mensaje para un valor que no se puede leer, por ejemplo una fecha con un formato incorrecto.
        /// </summary>
        public const string ValorNoValido = "El valor indicado no es válido.";

        /// <summary>
        /// Mensaje para una petición que llega sin cuerpo.
        /// </summary>
        public const string CuerpoObligatorio = "Hay que enviar un cuerpo en la petición.";
    }
}
