using AutoMapper;
using Database.Models;
using Repositories;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
        public interface IDocumentFileService
        {
            Task<DocumentFileDTO> Create(DocumentFileDTO payload);
            Task<IEnumerable<DocumentFileDTO>> GetAll();
            Task<DocumentFileDTO> GetById(int id);
            Task<DocumentFileDTO> Update(DocumentFileDTO payload);
            Task<int> Delete(int id);

        }

        public class DocumentFileService : IDocumentFileService
        {
            private readonly IDocumentFileRepository _repository;
            private readonly IMapper _mapper;
            public DocumentFileService(IDocumentFileRepository repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;

            }
            public async Task<DocumentFileDTO> Create(DocumentFileDTO payload)
            {

                var data = _mapper.Map<TblDocumentFile>(payload);
                try
                {
                    await _repository.AddAsync(data);
                    await _repository.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return _mapper.Map<DocumentFileDTO>(data);
            }

            public async Task<int> Delete(int id)
            {
                var foundItem = _repository.FirstOrDefault(x => x.Id == id);
                if (foundItem == null)
                {
                    throw new Exception("Item not found");
                }
                try
                {
                    _repository.Remove(foundItem);
                    await _repository.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return id;
            }

            public async Task<IEnumerable<DocumentFileDTO>> GetAll()
            {
                var data = _repository.GetAll();
                return _mapper.Map<IEnumerable<DocumentFileDTO>>(data);
            }

            public async Task<DocumentFileDTO> GetById(int id)
            {
                var data = await _repository.FirstOrDefaultAsync(x => x.Id == id);
                if (data == null)
                {
                    throw new Exception("Item not found");
                }
                return _mapper.Map<DocumentFileDTO>(data);
            }

            public async Task<DocumentFileDTO> Update(DocumentFileDTO payload)
            {
                var data = await _repository.FirstOrDefaultAsync(x => x.Id == payload.Id);
                if (data == null)
                {
                    throw new Exception($"{payload.Id} was not found");
                }
                //data.Title = payload.Title;
                //data.Active = payload.Active;
                await _repository.SaveChanges();
                return _mapper.Map<DocumentFileDTO>(data);
            }
        }
    }