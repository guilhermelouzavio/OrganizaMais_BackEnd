using Microsoft.Extensions.Configuration;
using SimpleBank.Domain.Interfaces.Repositories;
using SimpleBank.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleBank.Infra.Repositories;

namespace SimpleBank.Infra.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly string _connectionString;
        private IDbConnection? _connection; // Conexão que será compartilhada
        private IDbTransaction? _transaction; // Transação que será compartilhada

        // Propriedades de repositórios (lazy loading ou injetadas)
        public IUserRepository Users { get; private set; }
        public IFinancialAccountRepository FinancialAccounts { get; private set; }
        public ICategoryRepository Categories { get; private set; }
        public ITransactionRepository Transactions { get; private set; }

        public UnitOfWork(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new ArgumentNullException("DefaultConnection string not found.");

            // Inicia os repositórios (eles precisam da string de conexão)
            // Se você quiser que eles usem a mesma conexão/transação para operações do UnitOfWork,
            // você teria que passar a conexão/transação para eles, ou refatorar para eles serem "scoped".
            // Por enquanto, cada repositório vai abrir e fechar sua própria conexão em cada operação.
            // Para transações de verdade no Dapper, você precisaria passar a mesma IDbConnection e IDbTransaction.
            Users = new UserRepository(configuration);
            FinancialAccounts = new FinancialAccountRepository(configuration);
            Categories = new CategoryRepository(configuration);
            Transactions = new TransactionRepository(configuration);
        }

        // Este método é mais relevante para ORMs que controlam o estado.
        // Com Dapper, a "salvaguarda" é feita em cada operação SQL individualmente.
        // Se você quiser transações que abrangem múltiplas operações, você precisaria:
        // 1. Abrir uma única conexão aqui.
        // 2. Iniciar uma transação aqui.
        // 3. Passar a conexão e a transação para os construtores dos repositórios.
        // 4. Os repositórios usariam a conexão e transação compartilhadas.
        // 5. O CompleteAsync chamaria Commit() na transação.
        public async Task<int> CompleteAsync()
        {
            // Com o Dapper, cada operação de repositório já é atômica (salva na hora).
            // Se precisar de transações para múltiplas operações:
            // _transaction?.Commit();
            return await Task.FromResult(0); // Retorna 0 ou o número de linhas afetadas (se gerenciasse de outra forma)
        }

        // Dispose da conexão se ela foi criada e gerenciada pelo UnitOfWork
        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }
    }
}
