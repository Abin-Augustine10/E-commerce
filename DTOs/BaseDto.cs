namespace ShopZone.DTOs
{
    public abstract class BaseDto<T>
    {
        public T Id { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public abstract class BaseResponseDto<T> : BaseDto<T>
    {
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; } = true;
    }
}