using System;
using System.Linq;
using CarsCatalog.Models;

namespace CarsCatalog.Repository
{
    public interface ICarRepository
    {
        Car GetCarById(int? id);
        IQueryable<Car> GetCarsByModelId(int? modelId);
        OperationStatus AddCarWithDate(Car car, DateTime date);
        OperationStatus Add(Car car);
        OperationStatus Update(Car updatedCar);
        OperationStatus Delete(Car car);
    }
}