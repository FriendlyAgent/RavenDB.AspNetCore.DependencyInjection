using System.Linq;
using DependencyInjection.Sample.Models;
using Raven.Client.Documents.Indexes;

namespace DependencyInjection.Sample.Indexes
{
    public class Tests_ByName : AbstractIndexCreationTask<Test>
    {
        public Tests_ByName()
        {
            Map = tests => from t in tests
                           select new {
                               t.Name
                           };
        }
    }
}