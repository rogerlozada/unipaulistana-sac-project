namespace unipaulistana.model
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using unipaulistana.data;

    public class SolicitacaoRepository : ISolicitacaoRepository
    {
        public SolicitacaoRepository(ConexaoContext conexao)
        {
            this.conexao = conexao;
        }

        readonly ConexaoContext conexao;

        public int Adicionar(Solicitacao solicitacao)
        {
            string query = @"insert into dbo.solicitacao (DataDeCriacao, Descricao, ClienteID, DepartamentoID, UsuarioID) 
                                        values (@DataDeCriacao, @Descricao, @ClienteID, @DepartamentoID, @UsuarioID); SELECT SCOPE_IDENTITY();";
            
            var cmd = new SqlCommand(query, this.conexao.ObterConexao());
            cmd.Parameters.Add("@DataDeCriacao", SqlDbType.DateTime, 255).Value = solicitacao.DataDeCriacao;
            cmd.Parameters.Add("@Descricao", SqlDbType.VarChar, 8000).Value = solicitacao.Descricao;
            cmd.Parameters.Add("@ClienteID", SqlDbType.Int).Value = solicitacao.ClienteID;
            cmd.Parameters.Add("@DepartamentoID", SqlDbType.Int).Value = solicitacao.DepartamentoID;
            cmd.Parameters.Add("@UsuarioID", SqlDbType.Int).Value = solicitacao.UsuarioID;
            
            return cmd.ExecuteNonQuery();
        }

        public void Atualizar(Solicitacao solicitacao)
        {
            string query = string.Format(@"update dbo.solicitacao 
                                                set Descricao=@Descricao, 
                                                    ClienteID=@ClienteID, 
                                                    DepartamentoID=@DepartamentoID ,
                                                    UsuarioID=@UsuarioID, 
                                                where SolicitacaoID={0}", solicitacao.SolicitacaoID);
                                                
            var cmd = new SqlCommand(query, this.conexao.ObterConexao());
            cmd.Parameters.Add("@Descricao", SqlDbType.VarChar, 8000).Value = solicitacao.Descricao;
            cmd.Parameters.Add("@ClienteID", SqlDbType.Int).Value = solicitacao.ClienteID;
            cmd.Parameters.Add("@DepartamentoID", SqlDbType.Int).Value = solicitacao.DepartamentoID;
            cmd.Parameters.Add("@UsuarioID", SqlDbType.Int).Value = solicitacao.UsuarioID;
            cmd.ExecuteNonQuery();
        }

        public void Concluir(int solicitacaoID)
        {
            string query = string.Format(@"update dbo.solicitacao 
                                                set DataDeConclusao=@DataDeConclusao, 
                                                    Concluido=1 
                                                where SolicitacaoID={0}", solicitacaoID);
                                                
            var cmd = new SqlCommand(query, this.conexao.ObterConexao());
            cmd.Parameters.Add("@DataDeConclusao", SqlDbType.DateTime).Value = DateTime.Now;
            cmd.ExecuteNonQuery();
        }

        public void Excluir(int solicitacaoID)
        {
            string excluiSQL = string.Format(@"delete from solicitacao where SolicitacaoID={0}",solicitacaoID);
            var cmd = new SqlCommand();
            cmd.CommandText = excluiSQL ;
            cmd.Connection = this.conexao.ObterConexao();
            cmd.ExecuteNonQuery();
        }

        public Solicitacao ObterPorID(int solicitacaoID)
        {
            string sql = string.Format("select * from solicitacao where SolicitacaoID={0}", solicitacaoID);
            var cmd = new SqlCommand(sql, this.conexao.ObterConexao());
            SqlDataReader sqlDataReader = cmd.ExecuteReader();

            var solicitacao = new Solicitacao();
            while (sqlDataReader.Read())
            {
                solicitacao = new Solicitacao( Convert.ToInt32(sqlDataReader["SolicitacaoID"]),
                                                sqlDataReader["Descricao"].ToString(),
                                                Convert.ToDateTime(sqlDataReader["DataDeCriacao"]),
                                                sqlDataReader["DataDeConclusao"],
                                                Convert.ToBoolean(sqlDataReader["Concluido"]),
                                                Convert.ToInt32(sqlDataReader["ClienteID"]),
                                                Convert.ToInt32(sqlDataReader["departamentoID"]),
                                                Convert.ToInt32(sqlDataReader["UsuarioID"]));
            }
            return solicitacao;
        }

        public IEnumerable<Solicitacao> ObterTodos()
        {
           var cmd = new SqlCommand("select * from solicitacao", this.conexao.ObterConexao());
            SqlDataReader sqlDataReader = cmd.ExecuteReader();

            while (sqlDataReader.Read())
                yield return new Solicitacao( Convert.ToInt32(sqlDataReader["SolicitacaoID"]),
                                                sqlDataReader["Descricao"].ToString(),
                                                Convert.ToDateTime(sqlDataReader["DataDeCriacao"]),
                                                sqlDataReader["DataDeConclusao"],
                                                Convert.ToBoolean(sqlDataReader["Concluido"]),
                                                Convert.ToInt32(sqlDataReader["ClienteID"]),
                                                Convert.ToInt32(sqlDataReader["departamentoID"]),
                                                Convert.ToInt32(sqlDataReader["UsuarioID"]));

        }
    }
}
