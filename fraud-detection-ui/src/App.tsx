import { useEffect, useState } from 'react';
import type { CreateTransactionRequest, Transaction } from './types';
import {
  createTransaction,
  getTransactions,
  deleteTransaction,
} from './services/transactionApi';

function App() {
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [formData, setFormData] = useState<CreateTransactionRequest>({
    accountId: '',
    amount: '',
    country: '',
    accountHomeCountry: '',
    merchant: '',
    occurredAt: '',
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  useEffect(() => {
    loadTransactions();
  }, []);

  async function loadTransactions() {
    try {
      setLoading(true);
      setError(null);

      const data = await getTransactions();
      setTransactions(data);
    } catch (err) {
      console.log(err);
      setError('Could not load transactions.');
    } finally {
      setLoading(false);
    }
  }

  function handleChange(e: React.ChangeEvent<HTMLInputElement>) {
    const { name, value } = e.target;

    setFormData((prev) => ({
      ...prev,
      [name]: name === 'amount' ? (value === '' ? '' : Number(value)) : value,
    }));
  }

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();

    try {
      setSuccessMessage(null);
      setLoading(true);
      setError(null);

      if (
        !formData.accountId ||
        formData.amount === '' ||
        !formData.country ||
        !formData.accountHomeCountry ||
        !formData.merchant ||
        !formData.occurredAt
      ) {
        setError('Please fill out all fields.');
        return;
      }

      const createdTransaction = await createTransaction(formData);
      setSuccessMessage('Transaction submitted successfully.');

      setTransactions((prev) => [createdTransaction, ...prev]);

      setFormData({
        accountId: '',
        amount: '',
        country: '',
        accountHomeCountry: '',
        merchant: '',
        occurredAt: '',
      });
    } catch (err) {
      console.log(err);
      if (err instanceof Error) {
        setError(err.message);
      } else {
        setError('Could not create transaction.');
      }
    } finally {
      setLoading(false);
    }
  }

  async function handleDelete(id: number) {
    const confirmed = window.confirm(
      'Are you sure you want to delete this transaction?',
    );
    if (!confirmed) return;

    try {
      setError(null);
      setSuccessMessage(null);

      await deleteTransaction(id);

      setTransactions((prev) =>
        prev.filter((transaction) => transaction.id !== id),
      );
      setSuccessMessage('Transaction deleted successfully.');
    } catch (err) {
      if (err instanceof Error) {
        setError(err.message);
      } else {
        setError('Could not delete transaction.');
      }
    }
  }

  return (
    <>
      <div className="min-h-screen bg-gray-100 p-6">
        <div className="mx-auto max-w-4xl">
          <h1 className="mb-6 text-3xl font-bold">Fraud Detection</h1>

          {loading && <p className="mb-4">Loading...</p>}
          {error && <p className="mb-4 text-red-600">{error}</p>}
          {successMessage && (
            <p className="mb-4 text-green-600">{successMessage}</p>
          )}

          <form
            onSubmit={handleSubmit}
            className="mb-8 rounded-lg bg-white p-6 shadow"
          >
            <h2 className="mb-4 text-xl font-semibold">Submit Transaction</h2>

            <div className="grid gap-4 md:grid-cols-2">
              <input
                type="text"
                name="accountId"
                placeholder="Account ID"
                value={formData.accountId}
                onChange={handleChange}
                className="rounded border p-2"
              />
              <input
                type="number"
                name="amount"
                placeholder="Amount"
                value={formData.amount}
                onChange={handleChange}
                className="rounded border p-2"
              />
              <input
                type="text"
                name="country"
                placeholder="Country"
                value={formData.country}
                onChange={handleChange}
                className="rounded border p-2"
              />
              <input
                type="text"
                name="accountHomeCountry"
                placeholder="Account Home Country"
                value={formData.accountHomeCountry}
                onChange={handleChange}
                className="rounded border p-2"
              />
              <input
                type="text"
                name="merchant"
                placeholder="Merchant"
                value={formData.merchant}
                onChange={handleChange}
                className="rounded border p-2"
              />
              <input
                type="datetime-local"
                name="occurredAt"
                value={formData.occurredAt}
                onChange={handleChange}
                className="rounded border p-2"
              />
            </div>

            <button
              type="submit"
              className="mt-4 rounded bg-blue-600 px-4 py-2 text-white hover:bg-blue-700 hover:cursor-pointer"
            >
              Submit Transaction
            </button>
          </form>

          <div className="rounded-lg bg-white p-6 shadow">
            <h2 className="mb-4 text-xl font-semibold">Transactions</h2>

            {transactions.length === 0 ? (
              <p className="text-gray-600">No transactions found.</p>
            ) : (
              <div className="space-y-4">
                {transactions.map((transaction) => (
                  <div key={transaction.id} className="rounded border p-4">
                    <div className="flex items-center justify-between">
                      <h3 className="font-semibold">{transaction.merchant}</h3>
                      <div className="flex  items-end gap-2">
                        <span
                          className={`rounded px-3 py-1 text-sm font-medium ${
                            transaction.decision === 'Approved'
                              ? 'bg-green-100 text-green-700'
                              : transaction.decision === 'Suspicious'
                                ? 'bg-yellow-100 text-yellow-700'
                                : 'bg-red-100 text-red-700'
                          }`}
                        >
                          {transaction.decision}
                        </span>

                        <button
                          onClick={() => handleDelete(transaction.id)}
                          className="rounded bg-red-600 px-3 py-1 text-sm text-white hover:bg-red-700"
                        >
                          Delete
                        </button>
                      </div>
                    </div>

                    <p className="mt-2 text-sm text-gray-700">
                      Account: {transaction.accountId}
                    </p>
                    <p className="mt-2 text-sm text-gray-700">
                      Acmount: {transaction.amount}
                    </p>
                    <p className="mt-2 text-sm text-gray-700">
                      Country: {transaction.country}
                    </p>
                    <p className="mt-2 text-sm text-gray-700">
                      Reason: {transaction.reason}
                    </p>
                    <p className="mt-2 text-sm text-gray-700">
                      Occurred At:{' '}
                      {new Date(transaction.occurredAt).toLocaleString()}
                    </p>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      </div>
    </>
  );
}

export default App;
