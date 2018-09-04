namespace unipaulistana.model
{
    using System;
    using System.Collections.Generic;

    public interface IDepartamentoService
    {
        IEnumerable<Departamento> ObterTodos();
        Departamento ObterPorID(int departamentoID);
        void Adicionar(Departamento departamento);
        void Atualizar(Departamento  departamento);
        void Excluir(int departamentoID);
    } 
}
