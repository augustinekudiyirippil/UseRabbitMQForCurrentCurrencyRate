using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO
{
    public class CurrencyRatesResponseDto
    {
        public string Base { get; set; }
        public DateTime Date { get; set; }
        public List<CurrencyRateDto> Rates { get; set; }
    }
}
