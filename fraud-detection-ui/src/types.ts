export type FraudDecision = 'Approved' | 'Suspicious' | 'Fraud';

export interface Transaction {
  id: number;
  accountId: string;
  amount: number;
  country: string;
  merchant: string;
  occurredAt: string;
  decision: FraudDecision;
  reason: string;
}

export interface CreateTransactionRequest {
  accountId: string;
  amount: number | string;
  country: string;
  accountHomeCountry: string;
  merchant: string;
  occurredAt: string;
}
