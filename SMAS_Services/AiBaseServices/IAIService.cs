using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.AiBaseServices
{
    public interface IAIService
    {
        Task<string> AskAI(string prompt);
    }
}
