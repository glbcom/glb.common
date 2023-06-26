using System;
using System.ComponentModel.DataAnnotations;

namespace Glb.Common.Dto;

public record IdempotentRequestDto
{
    public required Guid IdempotencyId { get; set; }
}