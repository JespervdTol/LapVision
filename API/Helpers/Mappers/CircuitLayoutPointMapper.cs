using Contracts.App.DTO.GPS;
using Model.Entities;

namespace API.Helpers.Mappers
{
    public static class CircuitLayoutPointMapper
    {
        public static CircuitLayoutPointDTO ToDTO(this CircuitLayoutPoint point)
        {
            return new CircuitLayoutPointDTO
            {
                Latitude = point.Latitude,
                Longitude = point.Longitude,
                OrderIndex = point.OrderIndex
            };
        }

        public static CircuitLayoutPoint ToModel(this CircuitLayoutPointDTO dto, int circuitId)
        {
            return new CircuitLayoutPoint
            {
                CircuitID = circuitId,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                OrderIndex = dto.OrderIndex
            };
        }
    }
}