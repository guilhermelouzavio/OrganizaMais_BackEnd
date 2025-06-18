-- Script de carga para a tabela de Categorias padrão

-- Limpa a tabela antes de inserir, se quiser recomeçar do zero
-- CUIDADO: Isso vai apagar todas as categorias existentes!
-- DELETE FROM "Categories";
--select * from categories
-- Inserindo Categorias de Receita (Income)
INSERT INTO "categories" ("userid", "name", "description", "type", "createdat") VALUES
(NULL, 'Salário', 'Renda principal do trabalho', 'Income', NOW()),
(NULL, 'Freelance/Bicos', 'Renda de trabalhos autônomos ou extras', 'Income', NOW()),
(NULL, 'Rendimentos de Investimentos', 'Lucros de aplicações financeiras', 'Income', NOW()),
(NULL, 'Presente/Doação', 'Valores recebidos como presente ou doação', 'Income', NOW()),
(NULL, 'Outras Receitas', 'Qualquer entrada de dinheiro não categorizada', 'Income', NOW());

-- Inserindo Categorias de Despesa (Expense)
INSERT INTO "categories" ("userid", "name", "description", "type", "createdat") VALUES
(NULL, 'Moradia', 'Aluguel, condomínio, IPTU e despesas com residência', 'Expense', NOW()),
(NULL, 'Mercado', 'Compras de alimentos e produtos para casa', 'Expense', NOW()),
(NULL, 'Restaurantes/Bares', 'Gastos com alimentação fora de casa', 'Expense', NOW()),
(NULL, 'Combustível', 'Abastecimento de veículos', 'Expense', NOW()),
(NULL, 'Transporte Público', 'Passagens de ônibus, metrô, trem', 'Expense', NOW()),
(NULL, 'Aplicativos/Táxi', 'Gastos com Uber, 99 e táxis', 'Expense', NOW()),
(NULL, 'Manutenção Veicular', 'Conserto e manutenção de veículos', 'Expense', NOW()),

(NULL, 'Contas Fixas', 'Conta de consumo fixo geral como água,luz,gás e Internet/TV/Telefone', 'Expense', NOW()),
(NULL, 'Educação', 'Mensalidades de cursos, faculdade, materiais', 'Expense', NOW()),
(NULL, 'Saúde', 'Consultas médicas, exames, remédios, plano de saúde', 'Expense', NOW()),
(NULL, 'Lazer/Entretenimento', 'Cinema, shows, passeios, hobbies, assinaturas de streaming', 'Expense', NOW()),
(NULL, 'Compras', 'Aquisição de roupas, calçados, eletrônicos, etc.', 'Expense', NOW()),
(NULL, 'Dívidas/Empréstimos', 'Pagamento de parcelas de dívidas ou empréstimos', 'Expense', NOW()),
(NULL, 'Impostos/Taxas', 'IPVA, licenciamento, taxas governamentais', 'Expense', NOW()),
(NULL, 'Cuidados Pessoais', 'Cabeleireiro, manicure, produtos de higiene, academia', 'Expense', NOW()),
(NULL, 'Outras Despesas', 'Qualquer gasto não categorizado especificamente', 'Expense', NOW());