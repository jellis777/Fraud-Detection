import { render, screen } from '@testing-library/react';
import App from './App';

test('renders fraud detection heading', () => {
  render(<App />);
  expect(screen.getByText(/fraud detection/i)).toBeInTheDocument();
});
