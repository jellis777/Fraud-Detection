import type { CreateTransactionRequest, Transaction } from '../types';

const API_BASE_URL = 'http://localhost:5018';

export async function getTransactions(): Promise<Transaction[]> {
  const response = await fetch(`${API_BASE_URL}/api/transactions`);

  if (!response.ok) {
    let message = 'Failed to fetch transactions.';

    try {
      const errorData = await response.json();
      message = errorData.message ?? message;
    } catch {
      // use default message;
    }

    throw new Error(message);
  }

  return response.json();
}

export async function createTransaction(
  data: CreateTransactionRequest,
): Promise<Transaction> {
  const response = await fetch(`${API_BASE_URL}/api/transactions`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    let message = 'Failed to create transaction.';

    try {
      const errorData = await response.json();
      message = errorData.message ?? message;
    } catch {
      // Silently ignore parsing errors; use default message
    }

    throw new Error(message);
  }

  return response.json();
}

export async function deleteTransaction(id: number): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/api/transactions/${id}`, {
    method: 'DELETE',
  });

  if (!response.ok) {
    let message = 'Failed to delete transaction.';

    try {
      const errorData = await response.json();
      message = errorData.message ?? message;
    } catch {
      //
    }

    throw new Error(message);
  }
}
