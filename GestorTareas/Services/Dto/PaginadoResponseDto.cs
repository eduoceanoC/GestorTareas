namespace GestorTareas.Services.Dto
{
    public class PaginadoResponseDto<T>
    {
        public List<T> Datos { get; set; }
        public int TotalRegistros { get; set; }
        public int TotalPaginas { get; set; }
        public bool HayPaginaSiguiente { get; set; }
        public bool HayPaginaAnterior { get; set; }
    }
}
