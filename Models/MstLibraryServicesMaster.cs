using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class MstLibraryServicesMaster
{
    public int LibraryServiceId { get; set; }

    public string? LibraryServiceName { get; set; }

    public int FacultyId { get; set; }
}
