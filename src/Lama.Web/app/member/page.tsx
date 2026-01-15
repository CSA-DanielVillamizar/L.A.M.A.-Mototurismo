import { redirect } from 'next/navigation';

export const metadata = {
  title: 'Mi Portal | L.A.M.A.',
  description: 'Portal personal del miembro L.A.M.A.',
};

export default function MemberPage() {
  // Redirect to dashboard as default member portal landing
  redirect('/member/dashboard');
}
