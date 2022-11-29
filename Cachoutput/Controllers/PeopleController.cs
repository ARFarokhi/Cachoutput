using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Cachoutput.Controllers
{
    [ApiController]
    [Route("api/people")]
    public class PeopleController : ControllerBase
    {
        private static readonly List<Person> people = new()
        {
            new Person(1,"Sara",new List<string>(){"C#","Sql"}),
            new Person(2,"John",new List<string>(){"C#", "ElasticSearch"}),
            new Person(3,"Mark",new List<string>(){"Java","Html","CSS" }),
            new Person(4,"Helen",new List<string>(){".Net","Go"})
        };
        private readonly IOutputCacheStore _cache;
        public PeopleController(IOutputCacheStore cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// returns all persons
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [OutputCache(PolicyName = "PeoplePolicy")]
        public ActionResult<List<Person>> Get()
        {
            return Ok(people);
        }

        /// <summary>
        /// returns person by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [OutputCache(PolicyName = "ByIdCachePolicy")]
        public ActionResult<Person> Get(int id)
        {
            var person = people.FirstOrDefault(y => y.Id == id);
            return Ok(person);
        }

        /// <summary>
        /// delete person by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [OutputCache(PolicyName = "PeoplePolicy")]
        public async Task<ActionResult<Person>> DeletePerson(int id,CancellationToken token)
        {
            RemovePerson(id);
            await _cache.EvictByTagAsync("PeoplePolicy_Tag", token);
            return Ok();
        }

        private static void RemovePerson(int id)
        {
            var person = people.FirstOrDefault(y => y.Id == id);
            if (person is null)
                throw new Exception("Person not exists");

            people.Remove(person);
        }

        /// <summary>
        /// update Person by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="person"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> update(int id, Person person, CancellationToken token)
        {
            UpdatePerson(id, person);
            await _cache.EvictByTagAsync(id.ToString(), token);
            return Ok();
        }

        private static void UpdatePerson(int id, Person updatedPerson)
        {
            var person = people.FirstOrDefault(y => y.Id == id);
            if (person is null)
                throw new Exception("Person not exists");

            people.Remove(person);
            people.Add(updatedPerson);
        }
    }
}