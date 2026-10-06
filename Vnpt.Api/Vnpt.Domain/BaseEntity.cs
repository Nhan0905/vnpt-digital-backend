using System;
using System.Collections.Generic;
using System.Text;

namespace Vnpt.Domain
{
    // Class cơ sở chứa các thuộc tính dùng chung cho mọi bảng
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Đáp ứng yêu cầu Soft Delete trong đề cương
        public bool IsDeleted { get; set; } = false;
    }
}