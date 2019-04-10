using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestRubius.Models;

namespace TestRubius.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbEntities _context;

        public HomeController(DbEntities context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Получаем все записи (по умолчанию: порядок записей по убыванию даты)
            var records = await _context.Record.Include(r => r.Project)
                                    .OrderByDescending(r => r.DateTime)
                                    .ToListAsync();

            // Настраиваем параметры (текущая страница, порядок записей)
            var routeParams = new RouteParams
            {
                CurrentPage = 1,
                Order = 0
            };

            // Получаем количество страниц под записи (для настройки представления Pagination)
            if (records.Count % 5 == 0)
            {
                routeParams.PageCount = records.Count / 5;
            }
            else
            {
                routeParams.PageCount = records.Count / 5 + 1;
            }

            // Строготипизированная модель для представления
            var range = new RecordRange
            {
                RouteParams = routeParams,
                Records = records.Take(5)
            };

            return View(range);
        }

        public async Task<IActionResult> GetFilterRange(RouteParams routeParams)
        {
            // Получаем все записи
            IQueryable<Record> records = _context.Record;

            // Получаем количество записей
            var recordsCount = records.Count();

            // Фильтруем записи по проекту (если указан этот параметр)
            if (routeParams.ProjectId != Guid.Empty)
            {
                records = records.Where(r => r.ProjectId == routeParams.ProjectId);
            }

            // Фильтруем записи по начальной дате (если указан этот параметр)
            if (routeParams.StartDatetime != default(DateTime))
            {
                records = records.Where(r => r.DateTime >= routeParams.StartDatetime);
            }

            // Фильтруем записи по конечной дате (если указан этот параметр)
            if (routeParams.EndDatetime != default(DateTime))
            {
                records = records.Where(r => r.DateTime <= routeParams.EndDatetime);
            }

            // Сортируем записи по указанному порядку (по убыванию или по возрастанию даты)
            if (routeParams.Order == 1)
            {
                records = records.OrderBy(r => r.DateTime);
            }
            else
            {
                records = records.OrderByDescending(r => r.DateTime);
            }

            // Получаем указанное количество записей (если указан этот параметр)
            if (routeParams.RecordNumber != 0)
            {
                records = records.Take(routeParams.RecordNumber);
            }

            // Получаем список записей
            var recordsList = await records.ToListAsync();

            // Получаем количество записей в списке
            var recordsListCount = recordsList.Count;

            // Находим количество страниц (если неизвестен этот параметр)
            if (routeParams.PageCount == 0)
            {
                // Получаем количество страниц под записи (для настройки представления Pagination)
                if (recordsListCount % 5 == 0)
                {
                    routeParams.PageCount = recordsListCount / 5;
                }
                else
                {
                    routeParams.PageCount = recordsListCount + 1;
                }
            }

            // Строготипизированная модель для представления
            var range = new RecordRange
            {
                RouteParams = routeParams
            };

            // Находим индекс отсчета записей
            int index = routeParams.CurrentPage * 5 - 5;

            // Находим разницу количества записей
            int dif = recordsListCount - 5 * routeParams.CurrentPage;

            if (dif >= 0)
            {
                // На указанной странице ровно 5 записей
                range.Records = recordsList.GetRange(index, 5);
            }
            else if (dif < 0 && dif > -5)
            {
                // На указанной странице меньше 5 записей
                range.Records = recordsList.GetRange(index, 5 + dif);
            }
            else
            {
                // На указанной странице нет записей
                range.Records = recordsList;
            }

            return PartialView(range);
        }

        public async Task<IActionResult> Filter()
        {
            // Получаем все записи
            var projects = await _context.Project.ToListAsync();

            // Находим минимальную дату
            var date = await _context.Record.MinAsync(r => r.DateTime);

            // Настраиваем параметры (количество всего записей, начальную и конечную даты, порядок записей)
            var routeParams = new RouteParams
            {
                RecordNumber = _context.Record.Count(),
                StartDatetime = date,
                EndDatetime = DateTime.Now,
                Order = 0
            };

            // Получаем все проекты (для отображения в select)
            ViewBag.Projects = projects;

            return PartialView(routeParams);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            // Получаем все проекты
            var projects = await _context.Project.ToListAsync();

            return PartialView(projects);
        }

        [HttpPost]
        public int Add(Guid projectId, string name, string comment)
        {
            // Проверяем проект (новый или имеющийся)
            if (projectId == Guid.Empty)
            {
                // Создаем новый проект
                var project = new Project
                {
                    Id = Guid.NewGuid(),
                    Name = name
                };

                // Указываем guid нового проекта
                projectId = project.Id;

                // Добавляем новый проект в базу
                _context.Project.Add(project);
                _context.SaveChanges();
            }

            // Создаем новую запись
            var record = new Record
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                Comment = comment,
                DateTime = DateTime.Now
            };

            // Добавляем запись в базу
            _context.Record.AddAsync(record);
            _context.SaveChanges();

            return _context.Record.Count();
        }

        public int Delete(Guid id)
        {
            // Получаем запись для удаления
            var record = _context.Record.FirstOrDefault(r => r.Id == id);

            // Удаляем запись из базы
            _context.Record.Remove(record);
            _context.SaveChanges();

            return _context.Record.Count();
        }

        public IActionResult SetHeader()
        {
            // Настраиваем заголовок
            return PartialView("Header", _context.Record.Count());
        }
    }
}
